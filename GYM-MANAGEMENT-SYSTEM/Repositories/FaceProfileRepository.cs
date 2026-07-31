using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class FaceProfileRepository : IFaceProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public FaceProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FaceProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.FaceProfiles
                .FirstOrDefaultAsync(f => f.UserId == userId);
        }

        public async Task<IEnumerable<FaceProfile>> GetAllAsync()
        {
            return await _context.FaceProfiles
                .Include(f => f.ApplicationUser)
                .ToListAsync();
        }

        public async Task<FaceProfile> UpsertAsync(FaceProfile profile)
        {
            var existing = await _context.FaceProfiles
                .FirstOrDefaultAsync(f => f.UserId == profile.UserId);

            if (existing == null)
            {
                profile.CreatedAt = DateTime.UtcNow;
                profile.UpdatedAt = DateTime.UtcNow;
                _context.FaceProfiles.Add(profile);
            }
            else
            {
                existing.DescriptorJson = profile.DescriptorJson;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return existing ?? profile;
        }
    }
}