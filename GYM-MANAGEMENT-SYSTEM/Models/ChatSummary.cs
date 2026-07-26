namespace GYM_MANAGEMENT_SYSTEM.Models;

public class ChatSummary
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public string SummaryText { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}