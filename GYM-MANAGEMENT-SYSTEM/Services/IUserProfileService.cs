using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<UserProfile> CreateOrUpdateAsync(string userId, UserProfileEditViewModel model);

        Task UpdateAgeAsync(string userId, int age);
        Task UpdateAvatarAsync(string userId, string avatarPath);
    }
}