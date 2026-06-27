using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IMembershipService
    {
        Task<IEnumerable<Membership>> GetUserMembershipsAsync(string userId);
        Task<Membership?> GetActiveMembershipAsync(string userId);
        Task<Membership> RegisterMembershipAsync(MembershipRegistrationViewModel model);
        Task<Membership> RenewMembershipAsync(int membershipId);
        Task<bool> CancelMembershipAsync(int membershipId);
        Task<bool> IsUserEligibleForRegistrationAsync(string userId);
        Task<Membership?> GetByIdAsync(int id);
    }
}