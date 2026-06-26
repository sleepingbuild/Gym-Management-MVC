namespace GYM_MANAGEMENT_SYSTEM.Models;

public class Payment
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int? MembershipId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = "VNPay"; // VNPay, Cash

    public string Status { get; set; } = "Pending"; // Pending, Success, Failed

    public string TransactionId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Membership? Membership { get; set; }
}