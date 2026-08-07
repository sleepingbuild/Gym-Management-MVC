using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class PackageCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên gói tập")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên gói tập phải từ 3-100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số ngày")]
        [Range(1, 365, ErrorMessage = "Số ngày phải từ 1-365")]
        public int DurationDays { get; set; }

        public bool IsActive { get; set; } = true;
    }
}