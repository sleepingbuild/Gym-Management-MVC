using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMembershipPackageRepository _packageRepository;

        public MembershipService(
            IMembershipRepository membershipRepository,
            IMembershipPackageRepository packageRepository)
        {
            _membershipRepository = membershipRepository;
            _packageRepository = packageRepository;
        }

        public async Task<IEnumerable<Membership>> GetUserMembershipsAsync(string userId)
        {
            return await _membershipRepository.GetByUserIdAsync(userId);
        }

        public async Task<Membership?> GetActiveMembershipAsync(string userId)
        {
            return await _membershipRepository.GetActiveByUserIdAsync(userId);
        }

        public async Task<Membership> RegisterMembershipAsync(MembershipRegistrationViewModel model)
        {
            // Kiểm tra user đã có membership active chưa
            var activeMembership = await _membershipRepository.GetActiveByUserIdAsync(model.UserId);
            if (activeMembership != null)
            {
                throw new InvalidOperationException("Bạn đã có gói tập đang hoạt động. Vui lòng gia hạn hoặc hủy gói hiện tại.");
            }

            // Lấy package info
            var package = await _packageRepository.GetByIdAsync(model.MembershipPackageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            if (!package.IsActive)
            {
                throw new InvalidOperationException("Gói tập này hiện không khả dụng.");
            }

            var membership = new Membership
            {
                UserId = model.UserId,
                MembershipPackageId = model.MembershipPackageId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(package.DurationDays),
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            return await _membershipRepository.AddAsync(membership);
        }

        public async Task<Membership> RenewMembershipAsync(int membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            var package = await _packageRepository.GetByIdAsync(membership.MembershipPackageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            // Gia hạn từ ngày hiện tại
            membership.StartDate = DateTime.UtcNow;
            membership.EndDate = DateTime.UtcNow.AddDays(package.DurationDays);
            membership.Status = "Active";

            return await _membershipRepository.UpdateAsync(membership);
        }

        public async Task<bool> CancelMembershipAsync(int membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
                return false;

            membership.Status = "Cancelled";
            await _membershipRepository.UpdateAsync(membership);
            return true;
        }

        public async Task<bool> IsUserEligibleForRegistrationAsync(string userId)
        {
            var active = await _membershipRepository.GetActiveByUserIdAsync(userId);
            return active == null;
        }

        public async Task<Membership?> GetByIdAsync(int id)
        {
            return await _membershipRepository.GetByIdAsync(id);
        }
    }
}