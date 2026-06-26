namespace GYM_MANAGEMENT_SYSTEM.Models;

public class Booking
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int TrainerId { get; set; }

    public DateTime SessionDate { get; set; }

    public string TimeSlot { get; set; } = string.Empty; // "09:00 - 10:00"

    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Trainer Trainer { get; set; } = null!;
}