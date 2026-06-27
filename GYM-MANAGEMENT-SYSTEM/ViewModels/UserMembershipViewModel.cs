namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class UserMembershipViewModel
    {
        public int Id { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string StatusBadgeClass => Status switch
        {
            "Active" => "badge-fitness green",
            "Expired" => "badge-fitness red",
            "Cancelled" => "badge-fitness orange",
            _ => "badge-fitness dark"
        };

        public string DurationDisplay => DurationDays >= 30
            ? $"{DurationDays / 30} tháng"
            : $"{DurationDays} ngày";

        public string PriceFormatted => $"{Price:N0} VNĐ";

        public bool IsActive => Status == "Active";
        public bool IsExpired => Status == "Expired";
        public bool IsCancelled => Status == "Cancelled";

        public int DaysRemaining => (EndDate - DateTime.UtcNow).Days;

        public string DaysRemainingDisplay => DaysRemaining > 0
            ? $"Còn {DaysRemaining} ngày"
            : "Đã hết hạn";
    }
}