using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IWorkoutProgressService
    {
        Task<IEnumerable<WorkoutProgress>> GetUserProgressAsync(string userId);
        Task<IEnumerable<WorkoutProgress>> GetLatestProgressAsync(string userId, int count = 10);
        Task<WorkoutProgress?> GetLatestAsync(string userId);
        Task<WorkoutProgress?> GetProgressByIdAsync(int id);
        Task<WorkoutProgress> CreateProgressAsync(WorkoutCreateViewModel model);
        Task<WorkoutProgress> UpdateProgressAsync(WorkoutEditViewModel model);
        Task<bool> DeleteProgressAsync(int id);
        Task<IEnumerable<WorkoutProgress>> GetProgressByDateRangeAsync(string userId, DateTime fromDate, DateTime toDate);
        Task<WorkoutStatisticsViewModel> GetStatisticsAsync(string userId);
        Task<double> GetWeightChangeAsync(string userId, int days = 30);
        Task<double> GetBodyFatChangeAsync(string userId, int days = 30);
    }
}