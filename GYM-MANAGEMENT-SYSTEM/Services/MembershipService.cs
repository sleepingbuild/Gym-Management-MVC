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
            var existingMemberships = await _membershipRepository.GetByUserIdAsync(model.UserId);
            if (existingMemberships.Any(m => m.Status == "Pending"))
            {
                throw new InvalidOperationException("Bạn có một gói tập đang chờ thanh toán. Vui lòng hoàn tất thanh toán hoặc hủy gói đó trước khi đăng ký gói mới.");
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
                Status = "Pending",
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

            membership.EndDate = membership.EndDate.AddDays(package.DurationDays);
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
            var memberships = await _membershipRepository.GetByUserIdAsync(userId);
            return !memberships.Any(m => m.Status == "Pending");
        }

        public async Task<Membership?> GetByIdAsync(int id)
        {
            return await _membershipRepository.GetByIdAsync(id);
        }
    }
}