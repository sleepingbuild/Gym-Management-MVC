using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IFaceProfileService
    {
        Task SaveFaceAsync(string userId, float[] descriptor);
        Task<bool> HasFaceProfileAsync(string userId);
        Task<IEnumerable<KioskFaceProfileViewModel>> GetAllForKioskAsync();

        // Descriptor của riêng 1 user — dùng cho trang tự điểm danh (Trainer)
        Task<float[]?> GetDescriptorAsync(string userId);

        // Danh sách toàn bộ user (Admin/Trainer/Member) để Admin chọn đăng ký khuôn mặt hộ
        Task<IEnumerable<FaceEnrollableUserViewModel>> GetEnrollableUsersAsync();
    }
}