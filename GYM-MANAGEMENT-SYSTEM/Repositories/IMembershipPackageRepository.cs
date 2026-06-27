using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface IMembershipPackageRepository
    {
        Task<IEnumerable<MembershipPackage>> GetAllAsync();
        Task<IEnumerable<MembershipPackage>> GetActivePackagesAsync();
        Task<MembershipPackage?> GetByIdAsync(int id);
        Task<MembershipPackage> AddAsync(MembershipPackage package);
        Task<MembershipPackage> UpdateAsync(MembershipPackage package);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<int> CountAsync();
    }
}