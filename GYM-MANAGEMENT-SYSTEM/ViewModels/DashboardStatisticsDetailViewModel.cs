namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class DashboardStatisticsDetailViewModel
    {
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // User Statistics
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int NewUsersToday { get; set; }
        public int NewUsersThisWeek { get; set; }
        public int NewUsersThisMonth { get; set; }

        // Membership Statistics  
        public int TotalMemberships { get; set; }
        public int ActiveMemberships { get; set; }
        public int ExpiredMemberships { get; set; }
        public int ExpiringSoonMemberships { get; set; }
        public int NewMembershipsToday { get; set; }
        public int NewMembershipsThisWeek { get; set; }
        public int NewMembershipsThisMonth { get; set; }

        // Trainer Statistics
        public int TotalTrainers { get; set; }
        public int AvailableTrainers { get; set; }
        public int BusyTrainers { get; set; }

        // Booking Statistics
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }

        // Payment Statistics
        public int TotalPayments { get; set; }
        public int SuccessPayments { get; set; }
        public int FailedPayments { get; set; }
        public int PendingPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueThisWeek { get; set; }
        public decimal RevenueThisMonth { get; set; }

        // Workout Statistics
        public int TotalWorkoutRecords { get; set; }
        public int WorkoutRecordsToday { get; set; }
        public int WorkoutRecordsThisWeek { get; set; }
        public int WorkoutRecordsThisMonth { get; set; }
    }
}