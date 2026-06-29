using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class BookingCreateViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn huấn luyện viên")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày tập")]
        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khung giờ")]
        public string TimeSlot { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string Notes { get; set; } = string.Empty;

        // Display properties
        public string? TrainerName { get; set; }
        public string DateDisplay => SessionDate.ToString("dd/MM/yyyy");
        public string TimeDisplay => SessionDate.ToString("HH:mm");
    }
}