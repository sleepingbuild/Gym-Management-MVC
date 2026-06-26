namespace GYM_MANAGEMENT_SYSTEM.Models;

public class MembershipPackage
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}