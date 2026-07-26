using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _repository;

        public UserProfileService(IUserProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<UserProfile> CreateOrUpdateAsync(string userId, UserProfileEditViewModel model)
        {
            var existing = await _repository.GetByUserIdAsync(userId);

            if (existing == null)
            {
                var profile = new UserProfile
                {
                    UserId = userId,
                    Weight = model.Weight,
                    Height = model.Height,
                    Age = model.Age,
                    Goal = model.Goal
                };
                return await _repository.AddAsync(profile);
            }

            existing.Weight = model.Weight;
            existing.Height = model.Height;
            existing.Age = model.Age;
            existing.Goal = model.Goal;
            return await _repository.UpdateAsync(existing);
        }

        public async Task UpdateAgeAsync(string userId, int age)
        {
            var existing = await _repository.GetByUserIdAsync(userId);

            if (existing == null)
            {
                var profile = new UserProfile
                {
                    UserId = userId,
                    Age = age
                };
                await _repository.AddAsync(profile);
                return;
            }

            existing.Age = age;
            await _repository.UpdateAsync(existing);
        }
    }
}