namespace GYM_MANAGEMENT_SYSTEM.Models;

public class ChatHistory
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public string Answer { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; } = false;
}