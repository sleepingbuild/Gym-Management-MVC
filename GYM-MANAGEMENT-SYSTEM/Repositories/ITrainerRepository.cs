using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface ITrainerRepository
    {
        Task<IEnumerable<Trainer>> GetAllAsync();
        Task<IEnumerable<Trainer>> GetAvailableTrainersAsync();
        Task<Trainer?> GetByIdAsync(int id);
        Task<Trainer?> GetByUserIdAsync(string userId);
        Task<Trainer> AddAsync(Trainer trainer);
        Task<Trainer> UpdateAsync(Trainer trainer);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<int> CountAsync();
    }
}