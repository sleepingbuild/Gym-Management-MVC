using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int? MembershipId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Method { get; set; } = string.Empty; // VNPay, Cash, Bank

        [Required]
        public string Status { get; set; } = string.Empty; // Pending, Success, Failed

        public string TransactionId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("MembershipId")]
        public Membership? Membership { get; set; }
    }
}