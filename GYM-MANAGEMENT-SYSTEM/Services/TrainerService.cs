using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepository _repository;

        public TrainerService(ITrainerRepository repository)
        {
            _repository = repository;
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
            // Kiểm tra email duy nhất
            if (!await _repository.IsEmailUniqueAsync(model.Email))
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            // Kiểm tra user đã là trainer chưa
            if (!string.IsNullOrEmpty(model.UserId))
            {
                var existing = await _repository.GetByUserIdAsync(model.UserId);
                if (existing != null)
                {
                    throw new InvalidOperationException("Người dùng này đã là huấn luyện viên.");
                }
            }

            var trainer = new Trainer
            {
                UserId = model.UserId,
                FullName = model.FullName,
                Specialization = model.Specialization,
                Bio = model.Bio,
                Phone = model.Phone,
                Email = model.Email,
                IsAvailable = model.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.AddAsync(trainer);
        }

        public async Task<Trainer> UpdateTrainerAsync(TrainerEditViewModel model)
        {
            var trainer = await _repository.GetByIdAsync(model.Id);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }

            // Kiểm tra email duy nhất (trừ chính nó)
            if (!await _repository.IsEmailUniqueAsync(model.Email, model.Id))
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            trainer.FullName = model.FullName;
            trainer.Specialization = model.Specialization;
            trainer.Bio = model.Bio;
            trainer.Phone = model.Phone;
            trainer.Email = model.Email;
            trainer.IsAvailable = model.IsAvailable;

            return await _repository.UpdateAsync(trainer);
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
    }
}