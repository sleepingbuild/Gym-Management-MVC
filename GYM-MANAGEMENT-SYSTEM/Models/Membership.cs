using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class Membership
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int MembershipPackageId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("MembershipPackageId")]
        public MembershipPackage? MembershipPackage { get; set; }

        public ICollection<Payment>? Payments { get; set; }
    }
}