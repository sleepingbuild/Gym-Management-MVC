using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IFaceProfileService
    {
        Task SaveFaceAsync(string userId, float[] descriptor);
        Task<bool> HasFaceProfileAsync(string userId);
        Task<IEnumerable<KioskFaceProfileViewModel>> GetAllForKioskAsync();

        Task<float[]?> GetDescriptorAsync(string userId);

        Task<IEnumerable<FaceEnrollableUserViewModel>> GetEnrollableUsersAsync();

        Task<string?> FindMatchingUserIdAsync(float[] descriptor);

        Task<bool> VerifyOwnFaceAsync(string userId, float[] descriptor);
    }
}