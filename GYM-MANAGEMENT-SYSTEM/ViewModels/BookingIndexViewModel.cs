namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class BookingIndexViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string DateDisplay => SessionDate.ToString("dd/MM/yyyy");

        // FIX: SessionDate chỉ lưu NGÀY (giờ luôn là 00:00) — giờ tập thật sự
        // nằm ở field TimeSlot (VD: "14:00"), không phải phần giờ của SessionDate.
        public string TimeDisplay => TimeSlot;
        public string StatusBadgeClass => Status switch
        {
            "Pending" => "badge-fitness orange",
            "Confirmed" => "badge-fitness blue",
            "Completed" => "badge-fitness green",
            "Cancelled" => "badge-fitness red",
            "NoShow" or "PtNoShow" => "badge-fitness dark",
            _ => "badge-fitness dark"
        };
        public string StatusDisplay => Status switch
        {
            "Pending" => "Chờ xác nhận",
            "Confirmed" => "Đã xác nhận",
            "Completed" => "Đã hoàn thành",
            "Cancelled" => "Đã hủy",
            "NoShow" => "Member không đến",
            "PtNoShow" => "PT không đến",
            _ => Status
        };
    }
}