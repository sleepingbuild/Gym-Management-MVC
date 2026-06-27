namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class RenewalInfoViewModel
    {
        public int MembershipId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public DateTime CurrentEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public bool IsActive { get; set; }
        public int DaysUntilExpiry { get; set; }

        public string PriceFormatted => $"{Price:N0} VNĐ";
        public string DurationDisplay => DurationDays >= 30
            ? $"{DurationDays / 30} tháng"
            : $"{DurationDays} ngày";

        public string CurrentEndDateDisplay => CurrentEndDate.ToString("dd/MM/yyyy");
        public string NewEndDateDisplay => NewEndDate.ToString("dd/MM/yyyy");

        public string StatusDisplay => IsActive ? "Đang hoạt động" : "Đã hết hạn";
        public string StatusBadgeClass => IsActive ? "badge-fitness green" : "badge-fitness red";

        public string DaysUntilExpiryDisplay => DaysUntilExpiry > 0
            ? $"Còn {DaysUntilExpiry} ngày"
            : "Đã hết hạn";
    }
}