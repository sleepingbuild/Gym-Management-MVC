using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class MembershipRenewalService : IMembershipRenewalService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMembershipPackageRepository _packageRepository;

        public MembershipRenewalService(
            IMembershipRepository membershipRepository,
            IMembershipPackageRepository packageRepository)
        {
            _membershipRepository = membershipRepository;
            _packageRepository = packageRepository;
        }

        public async Task<RenewalInfoViewModel> GetRenewalInfoAsync(int membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            var package = membership.MembershipPackage;
            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin gói tập.");
            }

            var viewModel = new RenewalInfoViewModel
            {
                MembershipId = membership.Id,
                PackageName = package.Name,
                Price = package.Price,
                DurationDays = package.DurationDays,
                CurrentEndDate = membership.EndDate,
                NewEndDate = membership.EndDate.AddDays(package.DurationDays),
                IsActive = membership.Status == "Active",
                DaysUntilExpiry = (membership.EndDate - DateTime.UtcNow).Days
            };

            return viewModel;
        }

        public async Task<Membership> RenewMembershipAsync(int membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            if (!await CanRenewAsync(membershipId))
            {
                throw new InvalidOperationException("Gói tập này không thể gia hạn.");
            }

            var package = membership.MembershipPackage;
            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin gói tập.");
            }

            membership.EndDate = membership.EndDate.AddDays(package.DurationDays);
            membership.Status = "Active";

            return await _membershipRepository.UpdateAsync(membership);
        }

        public async Task<IEnumerable<Membership>> GetExpiringMembershipsAsync(int daysThreshold = 7)
        {
            var all = await _membershipRepository.GetAllAsync();
            var expiringSoon = all
                .Where(m => m.Status == "Active" &&
                           (m.EndDate - DateTime.UtcNow).Days <= daysThreshold &&
                           (m.EndDate - DateTime.UtcNow).Days >= 0)
                .OrderBy(m => m.EndDate);
            return expiringSoon;
        }

        public async Task<bool> CanRenewAsync(int membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
                return false;

            
            if (membership.Status != "Active" && membership.Status != "Expired")
                return false;

       
            if (membership.Status == "Expired" &&
                (DateTime.UtcNow - membership.EndDate).Days > 30)
                return false;

            return true;
        }
    }
}