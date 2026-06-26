using System.Text;
using System.Text.Json;

namespace GYM_MANAGEMENT_SYSTEM.Services;

public class AIService
{
    private readonly HttpClient _http;
    private const string ChatEndpoint = "http://localhost:5000/chat";

    public AIService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> AskAsync(string question, List<ChatMessage>? history = null)
    {
        var payload = new
        {
            message = question,
            history = history ?? new List<ChatMessage>()
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _http.PostAsync(ChatEndpoint, content);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ChatResponse>(body);
            return result?.reply ?? "Xin lỗi, tôi không thể trả lời câu hỏi này.";
        }
        catch (Exception ex)
        {
            return $"Lỗi kết nối AI server: {ex.Message}";
        }
    }
}

// ── DTOs ─────────────────────────────────────────────────────────────────────
public class ChatMessage
{
    public string role { get; set; } = "";
    public string content { get; set; } = "";
}

public class ChatResponse
{
    public string reply { get; set; } = "";
}