using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class WorkoutProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public double Weight { get; set; }

        public double Height { get; set; }

        public double BodyFatPercentage { get; set; }

        public string Notes { get; set; } = string.Empty;

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}