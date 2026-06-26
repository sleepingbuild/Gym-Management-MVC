using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? UserProfileId { get; set; }
        public UserProfile? UserProfile { get; set; }

        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}