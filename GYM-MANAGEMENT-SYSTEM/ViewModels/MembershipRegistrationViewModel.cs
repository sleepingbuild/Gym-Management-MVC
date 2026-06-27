using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class MembershipRegistrationViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn gói tập")]
        public int MembershipPackageId { get; set; }

        // Display properties
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string DurationDisplay => DurationDays >= 30
            ? $"{DurationDays / 30} tháng"
            : $"{DurationDays} ngày";
        public string PriceFormatted => $"{Price:N0} VNĐ";
    }
}