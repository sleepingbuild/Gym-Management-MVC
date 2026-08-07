namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class MyMembershipViewModel
    {
        public int Id { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string PackageDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Expired, Cancelled

        public bool IsPastEndDate => EndDate < DateTime.UtcNow;

        public int DaysRemaining => (EndDate - DateTime.UtcNow).Days;

        public string DaysRemainingDisplay => DaysRemaining > 0
            ? $"Còn {DaysRemaining} ngày"
            : "Đã hết hạn";

        public string PriceFormatted => $"{Price:N0} VNĐ";

        public string StatusDisplay
        {
            get
            {
                if (Status == "Cancelled") return "Đã hủy";
                if (Status == "Expired" || IsPastEndDate) return "Đã hết hạn";
                return "Đang hoạt động";
            }
        }

        public string StatusBadgeClass
        {
            get
            {
                if (Status == "Cancelled") return "badge-fitness red";
                if (Status == "Expired" || IsPastEndDate) return "badge-fitness dark";
                return "badge-fitness green";
            }
        }
    }
}