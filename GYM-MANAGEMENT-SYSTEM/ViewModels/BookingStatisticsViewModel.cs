namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class BookingStatisticsViewModel
    {
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public int PastBookings { get; set; }

        public List<BookingSummaryViewModel> RecentCompleted { get; set; } = new();

        // Helper properties
        public int ActiveBookings => PendingBookings + ConfirmedBookings;
        public double CompletionRate => TotalBookings > 0
            ? Math.Round((double)CompletedBookings / TotalBookings * 100, 1)
            : 0;
        public double CancellationRate => TotalBookings > 0
            ? Math.Round((double)CancelledBookings / TotalBookings * 100, 1)
            : 0;
    }

    public class BookingSummaryViewModel
    {
        public int Id { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public string DateDisplay => SessionDate.ToString("dd/MM/yyyy HH:mm");
        public string StatusBadgeClass => Status switch
        {
            "Pending" => "badge-fitness orange",
            "Confirmed" => "badge-fitness blue",
            "Completed" => "badge-fitness green",
            "Cancelled" => "badge-fitness red",
            _ => "badge-fitness dark"
        };
    }
}