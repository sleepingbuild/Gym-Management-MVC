using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class WorkoutCreateViewModel : IValidatableObject
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập cân nặng")]
        [Range(0, double.MaxValue, ErrorMessage = "Cân nặng không được âm")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chiều cao")]
        [Range(0, double.MaxValue, ErrorMessage = "Chiều cao không được âm")]
        public double Height { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tỷ lệ mỡ")]
        [Range(0, double.MaxValue, ErrorMessage = "Tỷ lệ mỡ không được âm")]
        public double BodyFatPercentage { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chỉ số cơ bắp")]
        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số cơ bắp không được âm")]
        public double MuscleMass { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vòng eo")]
        [Range(0, double.MaxValue, ErrorMessage = "Vòng eo không được âm")]
        public double WaistCircumference { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;

        // Display
        public string BMI => Height > 0 ? ((Weight / ((Height / 100) * (Height / 100))).ToString("F1")) : "0";
        public string BMICategory => double.TryParse(BMI, out var bmi) ? (bmi switch
        {
            < 18.5 => "Thiếu cân",
            < 25 => "Bình thường",
            < 30 => "Thừa cân",
            _ => "Béo phì"
        }) : "Chưa có";

        // Tất cả số liệu chỉ cần không âm (không giới hạn trên vì có thể có
        // nhiều mức đo khác nhau), nhưng chỉ cho phép tối đa 2 chữ số sau dấu phẩy.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var (value, memberName, label) in new (double, string, string)[]
            {
                (Weight, nameof(Weight), "Cân nặng"),
                (Height, nameof(Height), "Chiều cao"),
                (BodyFatPercentage, nameof(BodyFatPercentage), "Tỷ lệ mỡ"),
                (MuscleMass, nameof(MuscleMass), "Chỉ số cơ bắp"),
                (WaistCircumference, nameof(WaistCircumference), "Vòng eo"),
            })
            {
                if (Math.Round(value, 2) != value)
                {
                    yield return new ValidationResult(
                        $"{label} chỉ được tối đa 2 chữ số sau dấu phẩy.",
                        new[] { memberName });
                }
            }
        }
    }
}