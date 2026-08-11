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

        // ============================================================
        // "Dọn dẹp" trạng thái mỗi lần đọc dữ liệu gói tập của 1 user:
        // - Active mà đã qua EndDate  -> Expired
        // - Scheduled mà đã tới StartDate -> Active (gói xuống cấp tới lượt kích hoạt)
        // Cách này thay cho việc chạy 1 job nền định kỳ — đơn giản, đủ dùng
        // cho quy mô hệ thống hiện tại.
        // ============================================================
        private async Task RefreshStatusesAsync(string userId)
        {
            var now = DateTime.UtcNow;
            var memberships = await _membershipRepository.GetByUserIdAsync(userId);

            foreach (var m in memberships)
            {
                if (m.Status == "Active" && m.EndDate < now)
                {
                    m.Status = "Expired";
                    await _membershipRepository.UpdateAsync(m);
                }
                else if (m.Status == "Scheduled" && m.StartDate <= now)
                {
                    m.Status = "Active";
                    await _membershipRepository.UpdateAsync(m);
                }
            }
        }

        public async Task<IEnumerable<Membership>> GetUserMembershipsAsync(string userId)
        {
            await RefreshStatusesAsync(userId);
            return await _membershipRepository.GetByUserIdAsync(userId);
        }

        public async Task<Membership?> GetActiveMembershipAsync(string userId)
        {
            await RefreshStatusesAsync(userId);
            return await _membershipRepository.GetActiveByUserIdAsync(userId);
        }

        public async Task<Membership?> GetScheduledMembershipAsync(string userId)
        {
            await RefreshStatusesAsync(userId);
            var memberships = await _membershipRepository.GetByUserIdAsync(userId);
            return memberships.FirstOrDefault(m => m.Status == "Scheduled");
        }

        public async Task<Membership> RegisterMembershipAsync(MembershipRegistrationViewModel model)
        {
            await RefreshStatusesAsync(model.UserId);

            var existingMemberships = await _membershipRepository.GetByUserIdAsync(model.UserId);
            if (existingMemberships.Any(m => m.Status == "Pending"))
            {
                throw new InvalidOperationException("Bạn có một gói tập đang chờ thanh toán. Vui lòng hoàn tất thanh toán hoặc hủy gói đó trước khi đăng ký gói mới.");
            }

            if (existingMemberships.Any(m => m.Status == "Scheduled"))
            {
                throw new InvalidOperationException("Bạn đã có một gói tập được lên lịch chuyển sang sẵn. Vui lòng chờ gói đó kích hoạt hoặc hủy trước khi đăng ký gói khác.");
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

            var activeMembership = await _membershipRepository.GetActiveByUserIdAsync(model.UserId);

            // Chưa có gói active -> đăng ký bình thường như cũ
            if (activeMembership == null)
            {
                var newMembership = new Membership
                {
                    UserId = model.UserId,
                    MembershipPackageId = model.MembershipPackageId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(package.DurationDays),
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };
                return await _membershipRepository.AddAsync(newMembership);
            }

            var currentPackage = activeMembership.MembershipPackage
                ?? await _packageRepository.GetByIdAsync(activeMembership.MembershipPackageId);

            if (currentPackage == null)
            {
                throw new InvalidOperationException("Không xác định được gói tập hiện tại.");
            }

            if (currentPackage.Id == package.Id)
            {
                throw new InvalidOperationException("Bạn đang sử dụng gói tập này. Vui lòng dùng chức năng Gia hạn thay vì đăng ký lại.");
            }

            // NÂNG CẤP (giá mới >= giá hiện tại): thay thế ngay — hủy gói cũ,
            // tạo gói mới ở trạng thái Pending chờ thanh toán như luồng đăng ký
            // thông thường; sau khi thanh toán thành công gói mới sẽ Active.
            if (package.Price >= currentPackage.Price)
            {
                activeMembership.Status = "Cancelled";
                await _membershipRepository.UpdateAsync(activeMembership);

                var upgradedMembership = new Membership
                {
                    UserId = model.UserId,
                    MembershipPackageId = model.MembershipPackageId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(package.DurationDays),
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };
                return await _membershipRepository.AddAsync(upgradedMembership);
            }

            // XUỐNG CẤP (giá mới thấp hơn): KHÔNG đụng tới gói hiện tại — lên
            // lịch 1 gói mới bắt đầu đúng ngày gói hiện tại kết thúc.
            var scheduledMembership = new Membership
            {
                UserId = model.UserId,
                MembershipPackageId = model.MembershipPackageId,
                StartDate = activeMembership.EndDate,
                EndDate = activeMembership.EndDate.AddDays(package.DurationDays),
                Status = "Scheduled",
                CreatedAt = DateTime.UtcNow
            };
            return await _membershipRepository.AddAsync(scheduledMembership);
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

        public async Task<string> GetPackageActionLabelAsync(string userId, int packageId)
        {
            await RefreshStatusesAsync(userId);
            var activeMembership = await _membershipRepository.GetActiveByUserIdAsync(userId);

            if (activeMembership == null)
                return "Đăng ký";

            if (activeMembership.MembershipPackageId == packageId)
                return "Gia hạn";

            var targetPackage = await _packageRepository.GetByIdAsync(packageId);
            var currentPackage = activeMembership.MembershipPackage
                ?? await _packageRepository.GetByIdAsync(activeMembership.MembershipPackageId);

            if (targetPackage == null || currentPackage == null)
                return "Đăng ký";

            if (targetPackage.Price > currentPackage.Price)
                return "Nâng cấp";

            if (targetPackage.Price < currentPackage.Price)
                return "Xuống cấp";

            return "Đăng ký";
        }
    }
}