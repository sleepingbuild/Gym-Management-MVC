using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface IWorkoutProgressRepository
    {
        Task<IEnumerable<WorkoutProgress>> GetAllAsync();
        Task<IEnumerable<WorkoutProgress>> GetByUserIdAsync(string userId);
        Task<IEnumerable<WorkoutProgress>> GetLatestByUserIdAsync(string userId, int count = 10);
        Task<WorkoutProgress?> GetLatestAsync(string userId);
        Task<WorkoutProgress?> GetByIdAsync(int id);
        Task<WorkoutProgress> AddAsync(WorkoutProgress progress);
        Task<WorkoutProgress> UpdateAsync(WorkoutProgress progress);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<WorkoutProgress>> GetByDateRangeAsync(string userId, DateTime fromDate, DateTime toDate);
        Task<double> GetWeightChangeAsync(string userId, int days = 30);
        Task<double> GetBodyFatChangeAsync(string userId, int days = 30);
    }
}