using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int TrainerId { get; set; }

        [Required]
        public DateTime SessionDate { get; set; }

        [Required]
        public string TimeSlot { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      
        public DateTime? CheckInTime { get; set; }

        // "Manual" hoặc "Face"
        public string? CheckInMethod { get; set; }

        // Navigation properties
        [ForeignKey("TrainerId")]
        public Trainer? Trainer { get; set; }
    }
}