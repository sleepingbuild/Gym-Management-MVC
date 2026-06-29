namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class ScheduleIndexViewModel
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

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