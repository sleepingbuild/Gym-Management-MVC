using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class TrainerAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TrainerId { get; set; }


        [Required]
        public DateTime Date { get; set; }


        [Required]
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

        // Điểm danh tan ca — null nếu chưa check-out
        public DateTime? CheckOutTime { get; set; }

        [Required]
        public string Status { get; set; } = "Present";

        // "Manual" (nút bấm cũ) hoặc "Face" (điểm danh khuôn mặt).
        // Default "Manual" để không phá vỡ dữ liệu/luồng chấm công cũ.
        [Required]
        public string Method { get; set; } = "Manual";

        [StringLength(300, ErrorMessage = "Ghi chú không được quá 300 ký tự")]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("TrainerId")]
        public Trainer? Trainer { get; set; }
    }
}