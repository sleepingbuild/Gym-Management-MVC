using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class WorkoutEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập cân nặng")]
        [Range(0, 500, ErrorMessage = "Cân nặng từ 0-500 kg")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chiều cao")]
        [Range(0, 300, ErrorMessage = "Chiều cao từ 0-300 cm")]
        public double Height { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tỷ lệ mỡ")]
        [Range(0, 100, ErrorMessage = "Tỷ lệ mỡ từ 0-100%")]
        public double BodyFatPercentage { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chỉ số cơ bắp")]
        [Range(0, 100, ErrorMessage = "Chỉ số cơ bắp từ 0-100")]
        public double MuscleMass { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vòng eo")]
        [Range(0, 60, ErrorMessage = "Vòng eo từ 0-60 cm")]
        public double WaistCircumference { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }
}