namespace GYM_MANAGEMENT_SYSTEM.VNPay
{
    public class VNPayConfig
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string CurrCode { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string QueryUrl { get; set; } = string.Empty;
    }
}