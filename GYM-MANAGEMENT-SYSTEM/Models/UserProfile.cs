using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }  // int IDENTITY, tự tăng

        public string UserId { get; set; } = string.Empty;

        public double Weight { get; set; }

        public double Height { get; set; }

        public int Age { get; set; }

        public string Goal { get; set; } = string.Empty;

        // Đường dẫn tương đối tới ảnh đại diện, VD: /uploads/avatars/xxx.jpg
        [StringLength(300)]
        public string? AvatarPath { get; set; }

        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; }
    }
}