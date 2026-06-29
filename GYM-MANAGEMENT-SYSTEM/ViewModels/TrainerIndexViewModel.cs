namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    public class TrainerIndexViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StatusText => IsAvailable ? "Đang hoạt động" : "Tạm nghỉ";
        public string StatusBadgeClass => IsAvailable ? "badge-fitness green" : "badge-fitness red";
        public string ShortBio => Bio.Length > 100 ? Bio[..100] + "..." : Bio;
    }
}