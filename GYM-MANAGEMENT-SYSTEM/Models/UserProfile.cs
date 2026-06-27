using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class UserProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public string Goal { get; set; } = string.Empty;

        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; }
    }
}