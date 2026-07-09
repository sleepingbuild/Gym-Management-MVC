using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class PaymentCreateViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int MembershipId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Method { get; set; } = string.Empty;

        public string PaymentInfo { get; set; } = string.Empty;
    }
}