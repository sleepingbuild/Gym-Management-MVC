using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class ScheduleEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giờ bắt đầu")]
        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giờ kết thúc")]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Ghi chú không được quá 200 ký tự")]
        public string Notes { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Display properties
        public string DayDisplay => DayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ 2",
            DayOfWeek.Tuesday => "Thứ 3",
            DayOfWeek.Wednesday => "Thứ 4",
            DayOfWeek.Thursday => "Thứ 5",
            DayOfWeek.Friday => "Thứ 6",
            DayOfWeek.Saturday => "Thứ 7",
            DayOfWeek.Sunday => "Chủ nhật",
            _ => ""
        };
        public string TimeDisplay => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
        public string StatusDisplay => IsActive ? "Đang hoạt động" : "Đã tạm dừng";
        public string StatusBadgeClass => IsActive ? "badge-fitness green" : "badge-fitness red";
    }
}