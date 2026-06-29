using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class TrainerSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Ghi chú không được quá 200 ký tự")]
        public string Notes { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("TrainerId")]
        public Trainer? Trainer { get; set; }

        // Helper property
        public string TimeDisplay => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
        public string DayDisplay => DayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ 2",
            DayOfWeek.Tuesday => "Thứ 3",
            DayOfWeek.Wednesday => "Thứ 4",
            DayOfWeek.Thursday => "Thứ 5",
            DayOfWeek.Friday => "Thứ 6",
            DayOfWeek.Saturday => "Thứ 7",
            DayOfWeek.Sunday => "Chủ nhật",
            _ => ""
        };
    }
}