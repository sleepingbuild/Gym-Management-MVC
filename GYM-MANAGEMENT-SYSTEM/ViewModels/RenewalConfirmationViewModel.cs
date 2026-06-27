namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class RenewalConfirmationViewModel
    {
        public int MembershipId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }

        public string PriceFormatted => $"{Price:N0} VNĐ";
        public string DurationDisplay => DurationDays >= 30
            ? $"{DurationDays / 30} tháng"
            : $"{DurationDays} ngày";
    }
}