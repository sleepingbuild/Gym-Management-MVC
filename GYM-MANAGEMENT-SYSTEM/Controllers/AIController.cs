using GYM_MANAGEMENT_SYSTEM.AI.Services;
using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    private bool IsSessionEmpty(int sessionId) =>
        !_context.ChatHistories.Any(h => h.SessionId == sessionId);

    [HttpGet]
    public async Task<IActionResult> Chat(int? sessionId)
    {
        var userId = GetUserId();
        ChatSession? session;

        if (sessionId.HasValue)
        {
            // Người dùng chủ động bấm vào 1 session cụ thể trong sidebar
            session = _context.ChatSessions.FirstOrDefault(s => s.Id == sessionId && s.UserId == userId);
            if (session == null) return RedirectToAction("Chat");
        }
        else
        {
            // Không chỉ định session -> xác định session "mặc định" cho lần này
            var authResult = await HttpContext.AuthenticateAsync();
            var loginTime = authResult?.Properties?.IssuedUtc?.UtcDateTime ?? DateTime.MinValue;

            var lastSession = _context.ChatSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();

            bool isFromPreviousLogin = lastSession == null || lastSession.CreatedAt < loginTime;

            if (isFromPreviousLogin)
            {
                bool lastIsEmpty = lastSession != null && IsSessionEmpty(lastSession.Id);

                if (lastIsEmpty)
                {
                    // Session cũ vẫn còn rỗng -> tái sử dụng, chỉ "làm mới" thời gian
                    lastSession!.CreatedAt = DateTime.UtcNow;
                    lastSession.LastActivityAt = DateTime.UtcNow;
                    session = lastSession;
                }
                else
                {
                    // Session cũ đã có nội dung -> tạo phiên mới hoàn toàn
                    session = new ChatSession { UserId = userId, Title = "Cuộc trò chuyện mới" };
                    _context.ChatSessions.Add(session);
                }
                _context.SaveChanges();
            }
            else
            {
                // Vẫn trong cùng phiên đăng nhập, đã có session -> tiếp tục dùng session đó
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
            // Sidebar CHỈ hiện các session đã có ít nhất 1 tin nhắn - session rỗng không hiển thị
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

        // Nếu session hiện tại (mới nhất) vẫn đang rỗng -> không tạo thêm, dùng luôn nó
        if (lastSession != null && IsSessionEmpty(lastSession.Id))
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

        var answer = await GetAnswerAsync(question, sessionId);
        var savedId = SaveHistoryAndReturnId(question, answer, userId, sessionId);

        if (session.Title == "Cuộc trò chuyện mới")
        {
            session.Title = question.Length > 40 ? question.Substring(0, 40) + "..." : question;
        }
        session.LastActivityAt = DateTime.UtcNow;
        _context.SaveChanges();

        _ = Task.Run(() => SummarizeInBackgroundAsync(sessionId));

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

    private async Task<string> GetAnswerAsync(string question, int sessionId)
    {
        var (kbAnswer, source) = await _kb.SearchAnswerAsync(question);

        if (!string.IsNullOrEmpty(kbAnswer) && source.StartsWith("KB_Direct"))
            return kbAnswer;

        var history = BuildHistoryContext(sessionId);

        if (!string.IsNullOrEmpty(kbAnswer) && source.StartsWith("KB_Context"))
        {
            var prompt = $"Dựa vào thông tin sau, hãy trả lời câu hỏi của người dùng một cách tự nhiên:\n" +
                         $"Thông tin tham khảo: {kbAnswer}\nCâu hỏi: {question}";
            return await _aiClient.AskAsync(prompt, history);
        }

        var inScope = await IsGymRelatedAsync(question);
        if (!inScope)
            return OutOfScopeMessage;

        return await _aiClient.AskAsync(question, history);
    }

    private List<object> BuildHistoryContext(int sessionId)
    {
        var history = new List<object>();

        var summary = _context.ChatSummaries.FirstOrDefault(s => s.SessionId == sessionId);
        if (summary != null)
        {
            history.Add(new { role = "system", content = $"Tóm tắt hội thoại trước đó: {summary.SummaryText}" });
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
            var userId = scopedContext.ChatSessions.First(s => s.Id == sessionId).UserId;
            scopedContext.ChatSummaries.Add(new ChatSummary
            {
                UserId = userId,
                SessionId = sessionId,
                SummaryText = newSummaryText,
                UpdatedAt = DateTime.UtcNow
            });
        }

        foreach (var h in toSummarize) h.IsArchived = true;
        scopedContext.SaveChanges();
    }
}