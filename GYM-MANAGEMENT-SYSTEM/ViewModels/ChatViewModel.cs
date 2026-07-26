using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels;

public class ChatViewModel
{
    public int SessionId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? SummaryText { get; set; }
    public List<ChatHistory> Histories { get; set; } = new();
    public List<ChatSession> Sessions { get; set; } = new();
}