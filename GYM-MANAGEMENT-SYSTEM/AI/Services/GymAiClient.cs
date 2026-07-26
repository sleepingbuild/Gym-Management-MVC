namespace GYM_MANAGEMENT_SYSTEM.AI.Services;

public interface IGymAiClient
{
    Task<string> AskAsync(string question, List<object>? history = null);
    Task<float[]> EmbedAsync(string text);
}

public class GymAiClient : IGymAiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<GymAiClient> _logger;

    public GymAiClient(HttpClient http, ILogger<GymAiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<string> AskAsync(string question, List<object>? history = null)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/chat/stream", new
            {
                message = question,
                history = history ?? new List<object>()
            });
            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(text)
                ? "Xin lỗi, model chưa trả lời được câu hỏi này."
                : text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gọi AI model thất bại");
            return "Xin lỗi, hệ thống AI hiện không khả dụng. Vui lòng thử lại sau.";
        }
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var response = await _http.PostAsJsonAsync("/embed", new { text });
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EmbedReply>();
        return result?.vector ?? Array.Empty<float>();
    }

    private class EmbedReply { public float[] vector { get; set; } = Array.Empty<float>(); }
}