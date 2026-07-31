using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class WorkoutProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Cân nặng không được âm")]
        public double Weight { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Chiều cao không được âm")]
        public double Height { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tỷ lệ mỡ không được âm")]
        public double BodyFatPercentage { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số cơ bắp không được âm")]
        public double MuscleMass { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Vòng eo không được âm")]
        public double WaistCircumference { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        // Helper properties
        public double BMI => Height > 0 ? Math.Round(Weight / ((Height / 100) * (Height / 100)), 1) : 0;

        public string BMICategory => BMI switch
        {
            < 18.5 => "Thiếu cân",
            < 25 => "Bình thường",
            < 30 => "Thừa cân",
            _ => "Béo phì"
        };

        public string BMIStatus => BMI switch
        {
            < 18.5 => "warning",
            < 25 => "success",
            < 30 => "warning",
            _ => "danger"
        };
    }
}