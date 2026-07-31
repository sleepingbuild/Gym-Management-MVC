using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class ScheduleCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Vui lòng chọn huấn luyện viên")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày làm việc")]
        [DataType(DataType.Date)]
        public DateOnly WorkDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "Vui lòng nhập giờ bắt đầu")]
        [DataType(DataType.Time)]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giờ kết thúc")]
        [DataType(DataType.Time)]
        public TimeOnly EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Ghi chú không được quá 200 ký tự")]
        public string Notes { get; set; } = string.Empty;

        // Display properties
        public string? TrainerName { get; set; }
        public DayOfWeek DayOfWeek => WorkDate.DayOfWeek;
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (WorkDate < today)
            {
                yield return new ValidationResult(
                    "Không thể chọn ngày làm việc trong quá khứ.",
                    new[] { nameof(WorkDate) });
            }
            else if (WorkDate == today && StartTime < TimeOnly.FromDateTime(DateTime.Now))
            {
                yield return new ValidationResult(
                    "Giờ bắt đầu không được ở trong quá khứ so với thời điểm hiện tại.",
                    new[] { nameof(StartTime) });
            }
        }
    }
}