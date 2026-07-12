namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class DashboardChartViewModel
    {
        public List<ChartDataPoint> RevenueData { get; set; } = new();
        public List<ChartDataPoint> MembershipData { get; set; } = new();
        public List<ChartDataPoint> BookingData { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int Count { get; set; }
    }

    public class MonthlyRevenueViewModel
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int Count { get; set; }
    }

    public class MonthlyMembershipViewModel
    {
        public string Month { get; set; } = string.Empty;
        public int NewMemberships { get; set; }
        public int Renewals { get; set; }
        public int Cancellations { get; set; }
    }
}