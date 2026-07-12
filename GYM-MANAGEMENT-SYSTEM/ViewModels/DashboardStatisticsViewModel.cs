namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class DashboardStatisticsViewModel
    {
        // User Statistics
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int ActiveUsersThisMonth { get; set; }

        // Membership Statistics
        public int TotalMemberships { get; set; }
        public int ActiveMemberships { get; set; }
        public int ExpiredMemberships { get; set; }
        public int ExpiringSoonMemberships { get; set; }

        // Trainer Statistics
        public int TotalTrainers { get; set; }
        public int AvailableTrainers { get; set; }

        // Booking Statistics
        public int TotalBookings { get; set; }
        public int BookingsThisMonth { get; set; }
        public int PendingBookings { get; set; }
        public int CompletedBookings { get; set; }

        // Payment Statistics
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int TotalPayments { get; set; }
        public int SuccessPayments { get; set; }

        // Workout Statistics
        public int TotalWorkoutRecords { get; set; }
        public int WorkoutRecordsThisMonth { get; set; }

        // Helper properties
        public string TotalRevenueDisplay => $"{TotalRevenue:N0} VNĐ";
        public string RevenueThisMonthDisplay => $"{RevenueThisMonth:N0} VNĐ";
        public string TotalUsersDisplay => $"{TotalUsers:N0}";
        public string TotalMembershipsDisplay => $"{TotalMemberships:N0}";

        public double MembershipActiveRate => TotalMemberships > 0
            ? Math.Round((double)ActiveMemberships / TotalMemberships * 100, 1)
            : 0;

        public double BookingCompletionRate => TotalBookings > 0
            ? Math.Round((double)CompletedBookings / TotalBookings * 100, 1)
            : 0;

        public double PaymentSuccessRate => TotalPayments > 0
            ? Math.Round((double)SuccessPayments / TotalPayments * 100, 1)
            : 0;
    }
}