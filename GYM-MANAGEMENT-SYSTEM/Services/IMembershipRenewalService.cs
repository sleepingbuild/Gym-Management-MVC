using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IMembershipRenewalService
    {
        Task<RenewalInfoViewModel> GetRenewalInfoAsync(int membershipId);
        Task<Membership> RenewMembershipAsync(int membershipId);
        Task<IEnumerable<Membership>> GetExpiringMembershipsAsync(int daysThreshold = 7);
        Task<bool> CanRenewAsync(int membershipId);
    }
}