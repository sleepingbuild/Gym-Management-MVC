namespace GYM_MANAGEMENT_SYSTEM.Models;

public class WorkoutProgress
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public double Weight { get; set; }

    public double Height { get; set; }

    public double BMI => Height > 0 ? Math.Round(Weight / (Height * Height), 2) : 0;

    public double BodyFatPercentage { get; set; }

    public string Notes { get; set; } = string.Empty;

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}