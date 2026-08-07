using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class AdminBookingCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn thành viên")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn huấn luyện viên")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày tập")]
        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Vui lòng chọn khung giờ")]
        public string TimeSlot { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Ghi chú không được quá 200 ký tự")]
        public string Notes { get; set; } = string.Empty;
    }

    public class BookableMemberViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class ScheduleBookingViewModel
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public DateOnly WorkDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}