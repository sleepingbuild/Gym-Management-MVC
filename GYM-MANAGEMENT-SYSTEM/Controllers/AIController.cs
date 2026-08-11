using GYM_MANAGEMENT_SYSTEM.AI.Services;
using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GYM_MANAGEMENT_SYSTEM.Controllers;

public class AIController : Controller
{
    private readonly KnowledgeBaseService _kb;
    private readonly IGymAiClient _aiClient;
    private readonly ApplicationDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;
    private const int ARCHIVE_THRESHOLD = 20;
    private const int KEEP_RECENT = 10;
    private const string OutOfScopeMessage =
        "Xin lỗi, câu hỏi này không nằm trong phạm vi tư vấn của tôi. " +
        "Tôi chỉ hỗ trợ các câu hỏi về tập luyện gym, dinh dưỡng thể thao và sức khỏe thể chất liên quan đến việc tập luyện. " +
        "Bạn có câu hỏi nào khác về chủ đề này không?";

    public AIController(KnowledgeBaseService kb, IGymAiClient aiClient,
                         ApplicationDbContext context, IServiceScopeFactory scopeFactory)
    {
        _kb = kb;
        _aiClient = aiClient;
        _context = context;
        _scopeFactory = scopeFactory;
    }

    private string GetUserId() =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "guest";

    [HttpGet]
    public async Task<IActionResult> Chat(int? sessionId)
    {
        var userId = GetUserId();
        ChatSession? session;

        if (sessionId.HasValue)
        {
            session = _context.ChatSessions.FirstOrDefault(s => s.Id == sessionId && s.UserId == userId);
            if (session == null) return RedirectToAction("Chat");
        }
        else
        {
            var authResult = await HttpContext.AuthenticateAsync();
            var loginTime = authResult?.Properties?.IssuedUtc?.UtcDateTime ?? DateTime.MinValue;

            var lastSession = _context.ChatSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();

            bool isFromPreviousLogin = lastSession == null || lastSession.CreatedAt < loginTime;

            if (isFromPreviousLogin)
            {
                bool lastIsEmpty = lastSession != null && !_context.ChatHistories.Any(h => h.SessionId == lastSession.Id);
                if (lastIsEmpty)
                {
                    lastSession!.CreatedAt = DateTime.UtcNow;
                    lastSession.LastActivityAt = DateTime.UtcNow;
                    session = lastSession;
                }
                else
                {
                    session = new ChatSession { UserId = userId, Title = "Cuộc trò chuyện mới" };
                    _context.ChatSessions.Add(session);
                }
                _context.SaveChanges();
            }
            else
            {
                session = lastSession!;
            }
        }

        var summary = _context.ChatSummaries.FirstOrDefault(s => s.SessionId == session.Id);

        var model = new ChatViewModel
        {
            SessionId = session.Id,
            SummaryText = summary?.SummaryText,
            Histories = _context.ChatHistories
                .Where(h => h.SessionId == session.Id && !h.IsArchived)
                .OrderByDescending(x => x.CreatedAt)
                .Take(20)
                .ToList(),
            Sessions = _context.ChatSessions
                .Where(s => s.UserId == userId && _context.ChatHistories.Any(h => h.SessionId == s.Id))
                .OrderByDescending(s => s.LastActivityAt)
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewSession()
    {
        var userId = GetUserId();
        var lastSession = _context.ChatSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();

        if (lastSession != null && !_context.ChatHistories.Any(h => h.SessionId == lastSession.Id))
        {
            return RedirectToAction("Chat", new { sessionId = lastSession.Id });
        }

        var session = new ChatSession { UserId = userId, Title = "Cuộc trò chuyện mới" };
        _context.ChatSessions.Add(session);
        _context.SaveChanges();
        return RedirectToAction("Chat", new { sessionId = session.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChatApi([FromForm] string question, [FromForm] int sessionId)
    {
        var userId = GetUserId();
        var session = _context.ChatSessions.FirstOrDefault(s => s.Id == sessionId && s.UserId == userId);
        if (session == null) return BadRequest();

        var answer = await GetAnswerAsync(question, sessionId, userId);
        var savedId = SaveHistoryAndReturnId(question, answer, userId, sessionId);

        if (session.Title == "Cuộc trò chuyện mới")
        {
            session.Title = question.Length > 40 ? question.Substring(0, 40) + "..." : question;
        }
        session.LastActivityAt = DateTime.UtcNow;
        _context.SaveChanges();

        // 2 tác vụ nền độc lập: tóm tắt phiên hiện tại + cập nhật bộ nhớ dài hạn theo tài khoản
        _ = Task.Run(() => SummarizeInBackgroundAsync(sessionId));
        _ = Task.Run(() => UpdateUserMemoryAsync(userId, question, answer));

        var summary = _context.ChatSummaries.FirstOrDefault(s => s.SessionId == sessionId);

        return Json(new
        {
            answer,
            chatHistoryId = savedId,
            summaryText = summary?.SummaryText,
            sessionTitle = session.Title,
            createdAt = DateTime.UtcNow.ToLocalTime().ToString("o")
        });
    }

    private async Task<bool> IsGymRelatedAsync(string question)
    {
        var classifyPrompt =
            "Câu hỏi sau có liên quan đến gym, tập luyện thể hình, dinh dưỡng thể thao, " +
            "hoặc sức khỏe thể chất khi tập gym hay không? " +
            "Chỉ trả lời đúng 1 từ duy nhất: CO hoặc KHONG. Không giải thích gì thêm.\n" +
            $"Câu hỏi: \"{question}\"";

        var result = await _aiClient.AskAsync(classifyPrompt);
        return result.Trim().ToUpperInvariant().StartsWith("CO");
    }

    private (double? Height, double? Weight) TryExtractHeightWeight(string question)
    {
        var heightMatch = Regex.Match(question,
            @"cao\s*(?:là|khoảng|còn)?\s*(\d+(?:[.,]\d+)?)\s*(m|cm)?", RegexOptions.IgnoreCase);
        var weightMatch = Regex.Match(question,
            @"nặng\s*(?:là|thành|còn|chỉ|khoảng)?\s*(\d+(?:[.,]\d+)?)\s*(kg)?", RegexOptions.IgnoreCase);

        double? height = null, weight = null;
        if (heightMatch.Success)
        {
            var val = double.Parse(heightMatch.Groups[1].Value.Replace(',', '.'));
            var unit = heightMatch.Groups[2].Value.ToLower();
            height = (unit == "cm" || val > 3) ? val / 100.0 : val;
        }
        if (weightMatch.Success)
        {
            weight = double.Parse(weightMatch.Groups[1].Value.Replace(',', '.'));
        }
        return (height, weight);
    }

    private string BuildPersonalContext(double heightM, double weightKg)
    {
        var bmi = weightKg / (heightM * heightM);
        string category = bmi switch
        {
            < 18.5 => "thiếu cân, nên ưu tiên mục tiêu tăng cân/tăng cơ",
            < 25 => "cân nặng bình thường, có thể tập theo mục tiêu tăng cơ hoặc giữ dáng",
            < 30 => "thừa cân, nên kết hợp cardio với tập tạ để giảm mỡ",
            _ => "béo phì, nên ưu tiên giảm mỡ an toàn, cường độ tăng dần"
        };
        return $"Thông tin người dùng: cao {heightM:0.00}m, nặng {weightKg:0.#}kg, BMI = {bmi:0.0} ({category}).";
    }

    private async Task<string> GetAnswerAsync(string question, int sessionId, string userId)
    {
        var session = _context.ChatSessions.First(s => s.Id == sessionId);

        var (extractedHeight, extractedWeight) = TryExtractHeightWeight(question);
        double? height = extractedHeight ?? session.LastHeightM;
        double? weight = extractedWeight ?? session.LastWeightKg;

        if (extractedHeight.HasValue) session.LastHeightM = extractedHeight;
        if (extractedWeight.HasValue) session.LastWeightKg = extractedWeight;
        if (extractedHeight.HasValue || extractedWeight.HasValue) _context.SaveChanges();

        string? personalContext = (height.HasValue && weight.HasValue)
            ? BuildPersonalContext(height.Value, weight.Value)
            : null;

        var (kbAnswer, source) = await _kb.SearchAnswerAsync(question);
        var history = BuildHistoryContext(sessionId, userId);

        
        bool hasUserMemory = _context.UserMemories.Any(m => m.UserId == userId);

        if (personalContext != null)
        {
            var refText = !string.IsNullOrEmpty(kbAnswer) ? $"Thông tin tham khảo chung: {kbAnswer}\n" : "";
            var prompt = $"{refText}{personalContext}\n" +
                         $"Dựa vào thông tin trên, hãy trả lời câu hỏi sau, có nhắc cụ thể tới chỉ số BMI:\n" +
                         $"Câu hỏi: {question}";
            return await _aiClient.AskAsync(prompt, history);
        }
        if (!string.IsNullOrEmpty(kbAnswer) && source.StartsWith("KB_Direct") && !hasUserMemory)
            return kbAnswer;

        if (!string.IsNullOrEmpty(kbAnswer) &&
            (source.StartsWith("KB_Context") || (source.StartsWith("KB_Direct") && hasUserMemory)))
        {
            var prompt = $"Dựa vào thông tin sau, hãy trả lời câu hỏi của người dùng một cách tự nhiên, " +
                         $"có lưu ý tới thông tin cá nhân đã biết về người dùng (nếu liên quan):\n" +
                         $"Thông tin tham khảo: {kbAnswer}\nCâu hỏi: {question}";
            return await _aiClient.AskAsync(prompt, history);
        }

        var inScope = await IsGymRelatedAsync(question);
        if (!inScope) return OutOfScopeMessage;

        return await _aiClient.AskAsync(question, history);
    }
    // Gộp bộ nhớ dài hạn theo TÀI KHOẢN (mọi phiên) + tóm tắt phiên hiện tại + các lượt gần đây
    private List<object> BuildHistoryContext(int sessionId, string userId)
    {
        var history = new List<object>();

        var userMemory = _context.UserMemories.FirstOrDefault(m => m.UserId == userId);
        if (userMemory != null)
        {
            history.Add(new { role = "system", content = $"Thông tin đã biết về người dùng (từ các lần trò chuyện trước, mọi phiên): {userMemory.MemoryText}" });
        }

        var summary = _context.ChatSummaries.FirstOrDefault(s => s.SessionId == sessionId);
        if (summary != null)
        {
            history.Add(new { role = "system", content = $"Tóm tắt hội thoại trước đó trong phiên này: {summary.SummaryText}" });
        }

        var recent = _context.ChatHistories
            .Where(h => h.SessionId == sessionId && !h.IsArchived)
            .OrderByDescending(h => h.CreatedAt)
            .Take(KEEP_RECENT)
            .OrderBy(h => h.CreatedAt)
            .ToList();

        foreach (var h in recent)
        {
            history.Add(new { role = "user", content = h.Question });
            history.Add(new { role = "assistant", content = h.Answer });
        }

        return history;
    }

    private int SaveHistoryAndReturnId(string question, string answer, string userId, int sessionId)
    {
        var entry = new ChatHistory
        {
            UserId = userId,
            SessionId = sessionId,
            Question = question,
            Answer = answer,
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatHistories.Add(entry);
        _context.SaveChanges();
        return entry.Id;
    }

    // Tóm tắt PHIÊN hiện tại (giữ nguyên như trước)
    private async Task SummarizeInBackgroundAsync(int sessionId)
    {
        using var scope = _scopeFactory.CreateScope();
        var scopedContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var scopedAiClient = scope.ServiceProvider.GetRequiredService<IGymAiClient>();

        var unarchived = scopedContext.ChatHistories
            .Where(h => h.SessionId == sessionId && !h.IsArchived)
            .OrderBy(h => h.CreatedAt)
            .ToList();

        if (unarchived.Count <= ARCHIVE_THRESHOLD) return;

        var toSummarize = unarchived.Take(unarchived.Count - KEEP_RECENT).ToList();
        if (!toSummarize.Any()) return;

        var existingSummary = scopedContext.ChatSummaries.FirstOrDefault(s => s.SessionId == sessionId);
        var conversationText = string.Join("\n", toSummarize.Select(h => $"User: {h.Question}\nBot: {h.Answer}"));
        var prompt = existingSummary != null
            ? $"Đây là bản tóm tắt cuộc trò chuyện trước đó: {existingSummary.SummaryText}\n\n" +
              $"Hãy cập nhật bản tóm tắt trên, gộp thêm thông tin quan trọng từ đoạn hội thoại mới sau (ngắn gọn, tối đa 5 câu):\n{conversationText}"
            : $"Hãy tóm tắt ngắn gọn (tối đa 5 câu) nội dung chính của cuộc trò chuyện sau:\n{conversationText}";

        var newSummaryText = await scopedAiClient.AskAsync(prompt);

        if (existingSummary != null)
        {
            existingSummary.SummaryText = newSummaryText;
            existingSummary.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var uid = scopedContext.ChatSessions.First(s => s.Id == sessionId).UserId;
            scopedContext.ChatSummaries.Add(new ChatSummary
            {
                UserId = uid,
                SessionId = sessionId,
                SummaryText = newSummaryText,
                UpdatedAt = DateTime.UtcNow
            });
        }

        foreach (var h in toSummarize) h.IsArchived = true;
        scopedContext.SaveChanges();
    }

    // MỚI — cập nhật bộ nhớ dài hạn theo TÀI KHOẢN, chạy sau MỖI lượt chat, ở BẤT KỲ phiên nào
    private async Task UpdateUserMemoryAsync(string userId, string question, string answer)
    {
        using var scope = _scopeFactory.CreateScope();
        var scopedContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var scopedAiClient = scope.ServiceProvider.GetRequiredService<IGymAiClient>();

        var existing = scopedContext.UserMemories.FirstOrDefault(m => m.UserId == userId);

        var prompt = existing != null
            ?$"Đây là bộ nhớ hiện tại về người dùng:\n{existing.MemoryText}\n\n" +
             $"Đoạn hội thoại mới:\nUser: {question}\nBot: {answer}\n\n" +
             $"Nhiệm vụ: cập nhật bộ nhớ trên. CHỈ thêm thông tin CÁ NHÂN CỤ THỂ mới (chấn thương, mục tiêu, sở thích, chỉ số cơ thể...) nếu người dùng thực sự nhắc tới. " +
            $"KHÔNG suy diễn, KHÔNG bịa thêm nội dung không có trong hội thoại. " +
            $"Nếu đoạn hội thoại mới KHÔNG có thông tin cá nhân nào, giữ nguyên bộ nhớ cũ, không đổi 1 chữ nào.\n\n" +
            $"Ví dụ đúng: User nói 'tôi bị đau vai' → thêm dòng '- Đang bị đau vai, cần tránh bài tập vai nặng'.\n" +
            $"Ví dụ SAI cần tránh: tự bịa thông tin không được nhắc tới (ví dụ chế độ ăn) khi user không hề nói gì về việc đó."
          : $"...";

        var result = await scopedAiClient.AskAsync(prompt);

        Console.WriteLine($"[MEMORY DEBUG] UserId={userId}");
        Console.WriteLine($"[MEMORY DEBUG] Prompt gửi đi:\n{prompt}");
        Console.WriteLine($"[MEMORY DEBUG] Qwen trả về:\n{result}");

        if (result.Trim().ToUpperInvariant() == "KHONG") return;

        if (existing != null)
        {
            existing.MemoryText = result;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            scopedContext.UserMemories.Add(new UserMemory { UserId = userId, MemoryText = result, UpdatedAt = DateTime.UtcNow });
        }
        scopedContext.SaveChanges();
    }
}