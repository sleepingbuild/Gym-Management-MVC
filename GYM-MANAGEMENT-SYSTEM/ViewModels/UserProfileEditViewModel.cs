using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class UserProfileEditViewModel
    {
        [Range(20, 300, ErrorMessage = "Cân nặng phải từ 20 đến 300 kg")]
        public double Weight { get; set; }

        [Range(50, 250, ErrorMessage = "Chiều cao phải từ 50 đến 250 cm")]
        public double Height { get; set; }

        [Range(10, 90, ErrorMessage = "Tuổi phải từ 10 đến 90")]
        public int Age { get; set; }

        [StringLength(200, ErrorMessage = "Mục tiêu không được quá 200 ký tự")]
        public string Goal { get; set; } = string.Empty;
    }
}