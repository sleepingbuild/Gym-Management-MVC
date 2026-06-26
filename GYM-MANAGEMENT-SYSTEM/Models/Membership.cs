namespace GYM_MANAGEMENT_SYSTEM.Models;

public class Membership
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int MembershipPackageId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = "Active"; // Active, Expired, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public MembershipPackage MembershipPackage { get; set; } = null!;
}