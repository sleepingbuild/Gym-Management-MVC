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

        public TimeOnly? ShiftStartTime { get; set; }
        public TimeOnly? ShiftEndTime { get; set; }

        public DateTime? CheckOutTime { get; set; }
        public int? LateMinutes { get; set; }
        public int? EarlyLeaveMinutes { get; set; }

        public string ShiftDisplay =>
            (ShiftStartTime.HasValue && ShiftEndTime.HasValue)
                ? $"{ShiftStartTime:HH\\:mm} - {ShiftEndTime:HH\\:mm}"
                : "Chưa đặt ca";

        public string CheckInDisplay => CheckInTime.HasValue ? CheckInTime.Value.ToString("HH:mm") : "—";
        public string CheckOutDisplay => CheckOutTime.HasValue ? CheckOutTime.Value.ToString("HH:mm") : "—";

        public string ShiftStatusDisplay
        {
            get
            {
                if (!HasCheckedIn) return "Vắng mặt";
                if (!ShiftStartTime.HasValue || !ShiftEndTime.HasValue) return "Chưa đặt ca";

                var isLate = LateMinutes.GetValueOrDefault() > 0;
                var isEarly = EarlyLeaveMinutes.GetValueOrDefault() > 0;

                if (isLate && isEarly) return "Đi muộn & Về sớm";
                if (isLate) return "Đi muộn";
                if (isEarly) return "Về sớm";
                return "Đúng giờ";
            }
        }

        public string ShiftStatusBadgeClass => ShiftStatusDisplay switch
        {
            "Đúng giờ" => "badge-fitness green",
            "Đi muộn" => "badge-fitness orange",
            "Về sớm" => "badge-fitness orange",
            "Đi muộn & Về sớm" => "badge-fitness red",
            "Vắng mặt" => "badge-fitness dark",
            _ => "badge-fitness dark"
        };

        public string ShiftNoteDisplay
        {
            get
            {
                var parts = new List<string>();
                if (LateMinutes.GetValueOrDefault() > 0)
                {
                    parts.Add($"Đi muộn {LateMinutes} phút");
                }
                if (EarlyLeaveMinutes.GetValueOrDefault() > 0)
                {
                    parts.Add($"Về sớm {EarlyLeaveMinutes} phút");
                }
                return parts.Count > 0 ? string.Join(" · ", parts) : "—";
            }
        }
    }
}