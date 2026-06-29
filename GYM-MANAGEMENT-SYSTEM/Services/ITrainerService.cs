using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface ITrainerService
    {
        Task<IEnumerable<Trainer>> GetAllTrainersAsync();
        Task<IEnumerable<Trainer>> GetAvailableTrainersAsync();
        Task<Trainer?> GetTrainerByIdAsync(int id);
        Task<Trainer?> GetTrainerByUserIdAsync(string userId);
        Task<Trainer> CreateTrainerAsync(TrainerCreateViewModel model);
        Task<Trainer> UpdateTrainerAsync(TrainerEditViewModel model);
        Task<bool> DeleteTrainerAsync(int id);
        Task<bool> ToggleAvailabilityAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<int> GetTrainerCountAsync();
    }
}