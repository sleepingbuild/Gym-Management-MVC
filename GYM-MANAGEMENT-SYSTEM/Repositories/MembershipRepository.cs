using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly ApplicationDbContext _context;

        public MembershipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Membership>> GetAllAsync()
        {
            return await _context.Memberships
                .Include(m => m.MembershipPackage)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Membership>> GetByUserIdAsync(string userId)
        {
            return await _context.Memberships
                .Include(m => m.MembershipPackage)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Membership?> GetActiveByUserIdAsync(string userId)
        {
            return await _context.Memberships
                .Include(m => m.MembershipPackage)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.Status == "Active");
        }

        public async Task<Membership?> GetByIdAsync(int id)
        {
            return await _context.Memberships
                .Include(m => m.MembershipPackage)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Membership> AddAsync(Membership membership)
        {
            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task<Membership> UpdateAsync(Membership membership)
        {
            _context.Entry(membership).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var membership = await GetByIdAsync(id);
            if (membership == null)
                return false;

            _context.Memberships.Remove(membership);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasActiveMembershipAsync(string userId)
        {
            return await _context.Memberships
                .AnyAsync(m => m.UserId == userId && m.Status == "Active");
        }
    }
}