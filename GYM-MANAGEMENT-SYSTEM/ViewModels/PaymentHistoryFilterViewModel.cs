using System.ComponentModel.DataAnnotations;

namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class PaymentHistoryFilterViewModel
    {
        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Trạng thái")]
        public string? Status { get; set; }

        [Display(Name = "Tìm kiếm")]
        public string? SearchTerm { get; set; }

        public List<string> StatusOptions { get; set; } = new()
        {
            "Tất cả", "Pending", "Success", "Failed"
        };
    }
}