using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface IMembershipRepository
    {
        Task<IEnumerable<Membership>> GetAllAsync();
        Task<IEnumerable<Membership>> GetByUserIdAsync(string userId);
        Task<Membership?> GetActiveByUserIdAsync(string userId);
        Task<Membership?> GetByIdAsync(int id);
        Task<Membership> AddAsync(Membership membership);
        Task<Membership> UpdateAsync(Membership membership);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasActiveMembershipAsync(string userId);
    }
}