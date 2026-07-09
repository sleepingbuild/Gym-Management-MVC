using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class WorkoutProgressService : IWorkoutProgressService
    {
        private readonly IWorkoutProgressRepository _repository;

        public WorkoutProgressService(IWorkoutProgressRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<WorkoutProgress>> GetUserProgressAsync(string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<WorkoutProgress>> GetLatestProgressAsync(string userId, int count = 10)
        {
            return await _repository.GetLatestByUserIdAsync(userId, count);
        }

        public async Task<WorkoutProgress?> GetLatestAsync(string userId)
        {
            return await _repository.GetLatestAsync(userId);
        }

        public async Task<WorkoutProgress?> GetProgressByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<WorkoutProgress> CreateProgressAsync(WorkoutCreateViewModel model)
        {
            var progress = new WorkoutProgress
            {
                UserId = model.UserId,
                Weight = model.Weight,
                Height = model.Height,
                BodyFatPercentage = model.BodyFatPercentage,
                MuscleMass = model.MuscleMass,
                WaistCircumference = model.WaistCircumference,
                Notes = model.Notes,
                RecordedAt = DateTime.UtcNow
            };

            return await _repository.AddAsync(progress);
        }

        public async Task<WorkoutProgress> UpdateProgressAsync(WorkoutEditViewModel model)
        {
            var progress = await _repository.GetByIdAsync(model.Id);
            if (progress == null)
            {
                throw new KeyNotFoundException("Không tìm thấy dữ liệu tiến trình.");
            }

            progress.Weight = model.Weight;
            progress.Height = model.Height;
            progress.BodyFatPercentage = model.BodyFatPercentage;
            progress.MuscleMass = model.MuscleMass;
            progress.WaistCircumference = model.WaistCircumference;
            progress.Notes = model.Notes;

            return await _repository.UpdateAsync(progress);
        }

        public async Task<bool> DeleteProgressAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<WorkoutProgress>> GetProgressByDateRangeAsync(string userId, DateTime fromDate, DateTime toDate)
        {
            return await _repository.GetByDateRangeAsync(userId, fromDate, toDate);
        }

        public async Task<WorkoutStatisticsViewModel> GetStatisticsAsync(string userId)
        {
            var all = await _repository.GetByUserIdAsync(userId);
            var latest = all.FirstOrDefault();
            var first = all.LastOrDefault();

            var stats = new WorkoutStatisticsViewModel
            {
                TotalRecords = all.Count(),
                LatestWeight = latest?.Weight ?? 0,
                LatestBMI = latest?.BMI ?? 0,
                LatestBodyFat = latest?.BodyFatPercentage ?? 0,
                LatestMuscleMass = latest?.MuscleMass ?? 0,
                LatestWaist = latest?.WaistCircumference ?? 0,
                StartWeight = first?.Weight ?? 0,
                StartBMI = first?.BMI ?? 0,
                StartBodyFat = first?.BodyFatPercentage ?? 0
            };

            // Tính thay đổi
            stats.WeightChange = stats.StartWeight > 0 && stats.LatestWeight > 0
                ? Math.Round(stats.LatestWeight - stats.StartWeight, 1) : 0;
            stats.BMIChange = stats.StartBMI > 0 && stats.LatestBMI > 0
                ? Math.Round(stats.LatestBMI - stats.StartBMI, 1) : 0;
            stats.BodyFatChange = stats.StartBodyFat > 0 && stats.LatestBodyFat > 0
                ? Math.Round(stats.LatestBodyFat - stats.StartBodyFat, 1) : 0;

            // Tính gần đây (7 ngày)
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var recent = all.Where(w => w.RecordedAt >= sevenDaysAgo).ToList();
            stats.RecentRecords = recent.Count;

            // Cập nhật gần nhất
            stats.LastUpdated = latest?.RecordedAt ?? DateTime.UtcNow;

            return stats;
        }

        public async Task<double> GetWeightChangeAsync(string userId, int days = 30)
        {
            return await _repository.GetWeightChangeAsync(userId, days);
        }

        public async Task<double> GetBodyFatChangeAsync(string userId, int days = 30)
        {
            return await _repository.GetBodyFatChangeAsync(userId, days);
        }
    }
}