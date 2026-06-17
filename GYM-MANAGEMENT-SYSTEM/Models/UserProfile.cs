namespace GYM_MANAGEMENT_SYSTEM.Models;

public class UserProfile
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public double Weight { get; set; }

    public double Height { get; set; }

    public int Age { get; set; }

    public string Goal { get; set; } = string.Empty;
}