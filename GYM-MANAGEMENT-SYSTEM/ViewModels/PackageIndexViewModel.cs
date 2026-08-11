namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class PackageIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int? MaxSessionsPerWeek { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }


        // Helper properties
        public string PriceFormatted => $"{Price:N0} VNĐ";
        public string DurationDisplay => DurationDays >= 30
            ? $"{DurationDays / 30} tháng"
            : $"{DurationDays} ngày";
        public string StatusText => IsActive ? "Đang hoạt động" : "Đã khóa";
        public string ActionLabel { get; set; } = "Đăng ký";
        public string StatusBadgeClass => IsActive ? "badge-fitness green" : "badge-fitness red";

        public string BookingLimitDisplay => MaxSessionsPerWeek switch
        {
            null => "Không giới hạn",
            0 => "Không hỗ trợ đặt lịch với HLV",
            _ => $"Tối đa {MaxSessionsPerWeek} buổi/tuần"
        };
    }
}