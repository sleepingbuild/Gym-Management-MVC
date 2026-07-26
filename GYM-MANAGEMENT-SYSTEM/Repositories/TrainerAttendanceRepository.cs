using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class TrainerAttendanceRepository : ITrainerAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainerAttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TrainerAttendance?> GetByTrainerAndDateAsync(int trainerId, DateTime date)
        {
            var dateOnly = date.Date;
            return await _context.TrainerAttendances
                .FirstOrDefaultAsync(a => a.TrainerId == trainerId && a.Date == dateOnly);
        }

        public async Task<IEnumerable<TrainerAttendance>> GetByTrainerAsync(int trainerId)
        {
            return await _context.TrainerAttendances
                .Where(a => a.TrainerId == trainerId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainerAttendance>> GetByDateAsync(DateTime date)
        {
            var dateOnly = date.Date;
            return await _context.TrainerAttendances
                .Include(a => a.Trainer)
                .Where(a => a.Date == dateOnly)
                .ToListAsync();
        }

        public async Task<TrainerAttendance> AddAsync(TrainerAttendance attendance)
        {
            _context.TrainerAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }
    }
}