namespace GYM_MANAGEMENT_SYSTEM.Models;

public class Feedback
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Type { get; set; } = "General";   // "General" hoặc "AnswerRating"
    public int Rating { get; set; }                  // General: 1-5 sao | AnswerRating: 1 (thích) hoặc -1 (không thích)
    public string? Comment { get; set; }
    public int? ChatHistoryId { get; set; }           // chỉ có giá trị khi Type = "AnswerRating"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}