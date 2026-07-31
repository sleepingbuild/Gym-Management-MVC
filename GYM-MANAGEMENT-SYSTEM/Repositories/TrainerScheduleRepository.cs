using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class TrainerScheduleRepository : ITrainerScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainerScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrainerSchedule>> GetAllAsync()
        {
            return await _context.TrainerSchedules
                .Include(s => s.Trainer)
                .OrderBy(s => s.TrainerId)
                .ThenBy(s => s.WorkDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainerSchedule>> GetByTrainerIdAsync(int trainerId)
        {
            return await _context.TrainerSchedules
                .Include(s => s.Trainer)
                .Where(s => s.TrainerId == trainerId && s.IsActive)
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainerSchedule>> GetAvailableSlotsAsync(int trainerId, DateTime date)
        {
            // NOTE: now matches the exact calendar date (WorkDate) instead of just
            // the day-of-week, since schedules are date-specific going forward.
            var workDate = DateOnly.FromDateTime(date);
            return await _context.TrainerSchedules
                .Where(s => s.TrainerId == trainerId
                           && s.WorkDate == workDate
                           && s.IsActive)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainerSchedule>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, int? trainerId = null)
        {
            var query = _context.TrainerSchedules
                .Include(s => s.Trainer)
                .Where(s => s.WorkDate >= startDate && s.WorkDate <= endDate);

            if (trainerId.HasValue)
            {
                query = query.Where(s => s.TrainerId == trainerId.Value);
            }

            return await query
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<TrainerSchedule?> GetByIdAsync(int id)
        {
            return await _context.TrainerSchedules
                .Include(s => s.Trainer)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<TrainerSchedule> AddAsync(TrainerSchedule schedule)
        {
            schedule.CreatedAt = DateTime.UtcNow;
            _context.TrainerSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<TrainerSchedule> UpdateAsync(TrainerSchedule schedule)
        {
            _context.Entry(schedule).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await GetByIdAsync(id);
            if (schedule == null)
                return false;

            _context.TrainerSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsSlotAvailableAsync(int trainerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludeId = null)
        {
            var query = _context.TrainerSchedules
                .Where(s => s.TrainerId == trainerId
                           && s.DayOfWeek == dayOfWeek
                           && s.IsActive);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            var conflicts = await query.ToListAsync();
            return !conflicts.Any(s =>
                (startTime >= s.StartTime && startTime < s.EndTime) ||
                (endTime > s.StartTime && endTime <= s.EndTime) ||
                (startTime <= s.StartTime && endTime >= s.EndTime));
        }

        public async Task<bool> HasScheduleConflictAsync(int trainerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludeId = null)
        {
            return !await IsSlotAvailableAsync(trainerId, dayOfWeek, startTime, endTime, excludeId);
        }

        public async Task<bool> IsWorkDateSlotAvailableAsync(int trainerId, DateOnly workDate, TimeOnly startTime, TimeOnly endTime, int? excludeId = null)
        {
            var query = _context.TrainerSchedules
                .Where(s => s.TrainerId == trainerId
                           && s.WorkDate == workDate
                           && s.IsActive);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            var conflicts = await query.ToListAsync();
            return !conflicts.Any(s =>
                (startTime >= s.StartTime && startTime < s.EndTime) ||
                (endTime > s.StartTime && endTime <= s.EndTime) ||
                (startTime <= s.StartTime && endTime >= s.EndTime));
        }

        public async Task<bool> HasWorkDateScheduleConflictAsync(int trainerId, DateOnly workDate, TimeOnly startTime, TimeOnly endTime, int? excludeId = null)
        {
            return !await IsWorkDateSlotAvailableAsync(trainerId, workDate, startTime, endTime, excludeId);
        }
    }
}