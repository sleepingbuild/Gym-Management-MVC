namespace GYM_MANAGEMENT_SYSTEM.Models;

public class UserMemory
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MemoryText { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}