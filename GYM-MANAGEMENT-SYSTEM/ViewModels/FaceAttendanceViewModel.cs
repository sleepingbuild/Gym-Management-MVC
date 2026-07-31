namespace GYM_MANAGEMENT_SYSTEM.ViewModels
{
    // Gửi lên khi đăng ký / cập nhật khuôn mặt (tự đăng ký cho chính mình)
    public class FaceProfileSaveViewModel
    {
        public float[] Descriptor { get; set; } = Array.Empty<float>();
    }

    // Gửi lên khi Admin đăng ký hộ khuôn mặt cho người khác bằng ảnh tĩnh
    public class AdminFaceProfileSaveViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public float[] Descriptor { get; set; } = Array.Empty<float>();
    }

    // 1 dòng trong danh sách để Admin chọn người cần đăng ký khuôn mặt
    public class FaceEnrollableUserViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin" | "Trainer" | "Member" | "Member, Trainer"...
        public bool HasFaceProfile { get; set; }
    }

    // Dữ liệu tải về Kiosk để so khớp ngay trên trình duyệt
    public class KioskFaceProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public float[] Descriptor { get; set; } = Array.Empty<float>();
    }

    // Gửi lên khi Kiosk nhận diện được 1 khuôn mặt khớp
    public class FaceCheckInRequestViewModel
    {
        public string UserId { get; set; } = string.Empty;
    }

    // Kết quả trả về cho Kiosk hiển thị
    public class FaceCheckInResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FullName { get; set; }

        // "Trainer" hoặc "Member"
        public string? Role { get; set; }
        public DateTime? Time { get; set; }
    }
}