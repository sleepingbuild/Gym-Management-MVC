using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class TrainerRepository : ITrainerRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trainer>> GetAllAsync()
        {
            return await _context.Trainers
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trainer>> GetAvailableTrainersAsync()
        {
            return await _context.Trainers
                .Where(t => t.IsAvailable)
                .OrderBy(t => t.FullName)
                .ToListAsync();
        }

        public async Task<Trainer?> GetByIdAsync(int id)
        {
            return await _context.Trainers
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Trainer?> GetByUserIdAsync(string userId)
        {
            return await _context.Trainers
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<Trainer> AddAsync(Trainer trainer)
        {
            trainer.CreatedAt = DateTime.UtcNow;
            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();
            return trainer;
        }

        public async Task<Trainer> UpdateAsync(Trainer trainer)
        {
            _context.Entry(trainer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return trainer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var trainer = await GetByIdAsync(id);
            if (trainer == null)
                return false;

            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Trainers.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var query = _context.Trainers.Where(t => t.Email == email);
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Trainers.CountAsync();
        }
    }
}