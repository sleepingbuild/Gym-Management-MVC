namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class PaymentStatisticsViewModel
    {
        public int TotalPayments { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalSuccessAmount { get; set; }
        public int SuccessCount { get; set; }
        public int PendingCount { get; set; }
        public int FailedCount { get; set; }
        public double SuccessRate { get; set; }

        public List<PaymentSummaryViewModel> RecentPayments { get; set; } = new();

        public string TotalAmountDisplay => $"{TotalAmount:N0} VNĐ";
        public string TotalSuccessAmountDisplay => $"{TotalSuccessAmount:N0} VNĐ";
        public string SuccessRateDisplay => $"{SuccessRate}%";
    }

    public class PaymentSummaryViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string AmountDisplay => $"{Amount:N0} VNĐ";
        public string DateDisplay => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string StatusBadgeClass => Status switch
        {
            "Pending" => "badge-fitness orange",
            "Success" => "badge-fitness green",
            "Failed" => "badge-fitness red",
            _ => "badge-fitness dark"
        };
        public string MethodDisplay => Method switch
        {
            "VNPay" => "VNPay",
            "Cash" => "Tiền mặt",
            "Bank" => "Chuyển khoản",
            _ => Method
        };
    }
}