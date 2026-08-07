using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class TrainerEditViewModel
    {
        public int Id { get; set; }

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

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        
        public IFormFile? AvatarFile { get; set; }

        
        public string? CurrentAvatarPath { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}