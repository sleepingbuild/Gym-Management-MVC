using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class BookingEditViewModel
    {
        public int Id { get; set; }

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

        public string Status { get; set; } = string.Empty;

        // Display properties
        public string? TrainerName { get; set; }
        public string DateDisplay => SessionDate.ToString("dd/MM/yyyy");

        // FIX: SessionDate chỉ lưu NGÀY (giờ luôn là 00:00) — giờ tập thật sự
        // nằm ở field TimeSlot, không phải phần giờ của SessionDate.
        public string TimeDisplay => TimeSlot;
        public string StatusBadgeClass => Status switch
        {
            "Pending" => "badge-fitness orange",
            "Confirmed" => "badge-fitness blue",
            "Completed" => "badge-fitness green",
            "Cancelled" => "badge-fitness red",
            "NoShow" => "badge-fitness dark",
            _ => "badge-fitness dark"
        };
    }
}