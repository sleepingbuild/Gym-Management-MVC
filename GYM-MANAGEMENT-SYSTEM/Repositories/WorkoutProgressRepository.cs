using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class WorkoutProgressRepository : IWorkoutProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkoutProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutProgress>> GetAllAsync()
        {
            return await _context.WorkoutProgresses
                .OrderByDescending(w => w.RecordedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkoutProgress>> GetByUserIdAsync(string userId)
        {
            return await _context.WorkoutProgresses
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.RecordedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkoutProgress>> GetLatestByUserIdAsync(string userId, int count = 10)
        {
            return await _context.WorkoutProgresses
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.RecordedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<WorkoutProgress?> GetLatestAsync(string userId)
        {
            return await _context.WorkoutProgresses
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.RecordedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<WorkoutProgress?> GetByIdAsync(int id)
        {
            return await _context.WorkoutProgresses
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WorkoutProgress> AddAsync(WorkoutProgress progress)
        {
            progress.RecordedAt = DateTime.UtcNow;
            _context.WorkoutProgresses.Add(progress);
            await _context.SaveChangesAsync();
            return progress;
        }

        public async Task<WorkoutProgress> UpdateAsync(WorkoutProgress progress)
        {
            _context.Entry(progress).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return progress;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var progress = await GetByIdAsync(id);
            if (progress == null)
                return false;

            _context.WorkoutProgresses.Remove(progress);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WorkoutProgress>> GetByDateRangeAsync(string userId, DateTime fromDate, DateTime toDate)
        {
            return await _context.WorkoutProgresses
                .Where(w => w.UserId == userId && w.RecordedAt >= fromDate && w.RecordedAt <= toDate)
                .OrderBy(w => w.RecordedAt)
                .ToListAsync();
        }

        public async Task<double> GetWeightChangeAsync(string userId, int days = 30)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);
            var progresses = await _context.WorkoutProgresses
                .Where(w => w.UserId == userId && w.RecordedAt >= fromDate)
                .OrderBy(w => w.RecordedAt)
                .ToListAsync();

            if (progresses.Count < 2)
                return 0;

            var first = progresses.First();
            var last = progresses.Last();
            return Math.Round(last.Weight - first.Weight, 1);
        }

        public async Task<double> GetBodyFatChangeAsync(string userId, int days = 30)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);
            var progresses = await _context.WorkoutProgresses
                .Where(w => w.UserId == userId && w.RecordedAt >= fromDate)
                .OrderBy(w => w.RecordedAt)
                .ToListAsync();

            if (progresses.Count < 2)
                return 0;

            var first = progresses.First();
            var last = progresses.Last();
            return Math.Round(last.BodyFatPercentage - first.BodyFatPercentage, 1);
        }
    }
}