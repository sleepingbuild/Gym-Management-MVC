using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Booking>? Bookings { get; set; }
    }
}