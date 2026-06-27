using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class MembershipPackageRepository : IMembershipPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public MembershipPackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MembershipPackage>> GetAllAsync()
        {
            return await _context.MembershipPackages
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MembershipPackage>> GetActivePackagesAsync()
        {
            return await _context.MembershipPackages
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<MembershipPackage?> GetByIdAsync(int id)
        {
            return await _context.MembershipPackages
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<MembershipPackage> AddAsync(MembershipPackage package)
        {
            package.CreatedAt = DateTime.UtcNow;
            _context.MembershipPackages.Add(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task<MembershipPackage> UpdateAsync(MembershipPackage package)
        {
            _context.Entry(package).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var package = await GetByIdAsync(id);
            if (package == null)
                return false;

            _context.MembershipPackages.Remove(package);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MembershipPackages.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.MembershipPackages.Where(p => p.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.MembershipPackages.CountAsync();
        }
    }
}