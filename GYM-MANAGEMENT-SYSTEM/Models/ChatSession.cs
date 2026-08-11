namespace GYM_MANAGEMENT_SYSTEM.Models;

public class ChatSession
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = "Cuộc trò chuyện mới";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public double? LastHeightM { get; set; }
    public double? LastWeightKg { get; set; }
}