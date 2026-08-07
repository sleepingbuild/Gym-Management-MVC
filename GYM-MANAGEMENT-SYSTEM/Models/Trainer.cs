using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên huấn luyện viên")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên phải từ 2-100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Chuyên môn không được quá 200 ký tự")]
        public string Specialization { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Tiểu sử không được quá 500 ký tự")]
        public string Bio { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        // Ngày sinh — tuổi được tính so với CreatedAt (ngày đăng ký), không
        // phải so với ngày hiện tại, nên không tự "già thêm" theo thời gian.
        public DateTime? DateOfBirth { get; set; }

        // Đường dẫn tương đối tới ảnh đại diện, VD: /uploads/avatars/xxx.jpg
        [StringLength(300)]
        public string? AvatarPath { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Booking>? Bookings { get; set; }
    }
}