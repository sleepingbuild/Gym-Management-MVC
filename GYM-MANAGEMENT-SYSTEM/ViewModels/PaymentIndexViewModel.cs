namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class PaymentIndexViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int? MembershipId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string PaymentInfo { get; set; } = string.Empty;
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
        public string StatusDisplay => Status switch
        {
            "Pending" => "Đang xử lý",
            "Success" => "Thành công",
            "Failed" => "Thất bại",
            _ => Status
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