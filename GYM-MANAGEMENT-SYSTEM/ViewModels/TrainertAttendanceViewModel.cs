namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    // Trang Chấm công của Trainer 
    public class TrainerAttendanceStatusViewModel
    {
        public bool HasCheckedInToday { get; set; }
        public DateTime? TodayCheckInTime { get; set; }
        public List<TrainerAttendanceHistoryItem> RecentHistory { get; set; } = new();
    }

    public class TrainerAttendanceHistoryItem
    {
        public DateTime Date { get; set; }
        public DateTime CheckInTime { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    // Trang Admin 
    public class AdminAttendanceReportViewModel
    {
        public DateTime Date { get; set; }
        public int TotalTrainers { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public List<AdminAttendanceRowViewModel> Rows { get; set; } = new();
    }

    public class AdminAttendanceRowViewModel
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public bool HasCheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}