namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class TrainerDashboardViewModel
    {
        public string TrainerName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int TodaySessionsCount { get; set; }
        public int UpcomingSessionsCount { get; set; }
        public int PendingConfirmCount { get; set; }
        public List<TrainerBookingViewModel> TodaySessions { get; set; } = new();
    }

    public class TrainerStudentViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Age { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public string? Goal { get; set; }
        public int TotalSessions { get; set; }
        public DateTime LastSessionDate { get; set; }
    }

    public class TrainerBookingViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public string StatusBadgeClass => Status switch
        {
            "Pending" => "badge-fitness orange",
            "Confirmed" => "badge-fitness blue",
            "Completed" => "badge-fitness green",
            "Cancelled" => "badge-fitness red",
            _ => "badge-fitness"
        };
    }

    
    public class TrainerBookingDetailViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int TrainerId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public int? StudentAge { get; set; }
        public double? StudentWeight { get; set; }
        public double? StudentHeight { get; set; }
        public string? StudentGoal { get; set; }
        public DateTime SessionDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public string StatusBadgeClass => Status switch
        {
            "Pending" => "badge-fitness orange",
            "Confirmed" => "badge-fitness blue",
            "Completed" => "badge-fitness green",
            "Cancelled" => "badge-fitness red",
            _ => "badge-fitness"
        };
    }

    
    public class TrainerStudentProgressViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool HasRecords { get; set; }
        public double LatestWeight { get; set; }
        public double LatestHeight { get; set; }
        public double LatestBMI { get; set; }
        public string LatestBMICategory { get; set; } = string.Empty;
        public string LatestBMIStatus { get; set; } = string.Empty;
        public double LatestBodyFatPercentage { get; set; }
        public double LatestMuscleMass { get; set; }
        public DateTime? RecordedAt { get; set; }
    }
}