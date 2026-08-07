using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        
        public const string DefaultTrainerPassword = "Trainer123";

        public TrainerService(
            ITrainerRepository repository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<Trainer>> GetAllTrainersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Trainer>> GetAvailableTrainersAsync()
        {
            return await _repository.GetAvailableTrainersAsync();
        }

        public async Task<Trainer?> GetTrainerByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Trainer?> GetTrainerByUserIdAsync(string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<Trainer> CreateTrainerAsync(TrainerCreateViewModel model)
        {
            if (!await _repository.IsEmailUniqueAsync(model.Email))
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            var createdAt = DateTime.UtcNow;

            if (CalculateAge(model.DateOfBirth, createdAt) < 18)
            {
                throw new InvalidOperationException("Huấn luyện viên phải từ 18 tuổi trở lên.");
            }

            var userId = model.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                userId = await CreateTrainerAccountAsync(model.Email, model.FullName, createdAt);
            }
            else
            {
                var existing = await _repository.GetByUserIdAsync(userId);
                if (existing != null)
                {
                    throw new InvalidOperationException("Người dùng này đã là huấn luyện viên.");
                }
            }

            var trainer = new Trainer
            {
                UserId = userId,
                FullName = model.FullName,
                Specialization = model.Specialization,
                Bio = model.Bio,
                Phone = model.Phone,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                IsAvailable = model.IsAvailable,
                CreatedAt = createdAt
            };

            return await _repository.AddAsync(trainer);
        }

        private async Task<string> CreateTrainerAccountAsync(string email, string fullName, DateTime createdAt)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "Email này đã có tài khoản đăng nhập trong hệ thống. Vui lòng dùng email khác hoặc liên hệ Admin để liên kết tài khoản.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true,
                CreatedAt = createdAt
            };

            var result = await _userManager.CreateAsync(user, DefaultTrainerPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể tạo tài khoản đăng nhập cho huấn luyện viên: {errors}");
            }

            if (!await _roleManager.RoleExistsAsync("Trainer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Trainer"));
            }

            await _userManager.AddToRoleAsync(user, "Trainer");

            return user.Id;
        }

        public async Task<Trainer> UpdateTrainerAsync(TrainerEditViewModel model)
        {
            var trainer = await _repository.GetByIdAsync(model.Id);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }


            if (!await _repository.IsEmailUniqueAsync(model.Email, model.Id))
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }


            if (CalculateAge(model.DateOfBirth, trainer.CreatedAt) < 18)
            {
                throw new InvalidOperationException("Huấn luyện viên phải từ 18 tuổi trở lên (tính theo ngày đăng ký).");
            }

            trainer.FullName = model.FullName;
            trainer.Specialization = model.Specialization;
            trainer.Bio = model.Bio;
            trainer.Phone = model.Phone;
            trainer.Email = model.Email;
            trainer.DateOfBirth = model.DateOfBirth;
            trainer.IsAvailable = model.IsAvailable;

            var updated = await _repository.UpdateAsync(trainer);
            await SyncApplicationUserFullNameAsync(trainer.UserId, trainer.FullName);
            return updated;
        }

        public async Task<Trainer> UpdateOwnProfileAsync(int trainerId, TrainerProfileEditViewModel model)
        {
            var trainer = await _repository.GetByIdAsync(trainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }

            trainer.FullName = model.FullName;
            trainer.Specialization = model.Specialization;
            trainer.Bio = model.Bio;
            trainer.Phone = model.Phone;

            var updated = await _repository.UpdateAsync(trainer);
            await SyncApplicationUserFullNameAsync(trainer.UserId, trainer.FullName);
            return updated;
        }

        private async Task SyncApplicationUserFullNameAsync(string userId, string fullName)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && user.FullName != fullName)
            {
                user.FullName = fullName;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<bool> DeleteTrainerAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ToggleAvailabilityAsync(int id)
        {
            var trainer = await _repository.GetByIdAsync(id);
            if (trainer == null)
                return false;

            trainer.IsAvailable = !trainer.IsAvailable;
            await _repository.UpdateAsync(trainer);
            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            return await _repository.IsEmailUniqueAsync(email, excludeId);
        }

        public async Task<int> GetTrainerCountAsync()
        {
            return await _repository.CountAsync();
        }

        public async Task UpdateAvatarAsync(int trainerId, string avatarPath)
        {
            var trainer = await _repository.GetByIdAsync(trainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }

            trainer.AvatarPath = avatarPath;
            await _repository.UpdateAsync(trainer);
        }

        private static int CalculateAge(DateTime dateOfBirth, DateTime asOf)
        {
            var age = asOf.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > asOf.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}