using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IMembershipPackageService _packageService;
        private readonly IMembershipRenewalService _renewalService;

        public MembershipController(
            IMembershipService membershipService,
            IMembershipPackageService packageService,
            IMembershipRenewalService renewalService)
        {
            _membershipService = membershipService;
            _packageService = packageService;
            _renewalService = renewalService;
        }

        // GET: /Membership
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var memberships = await _membershipService.GetUserMembershipsAsync(userId);
            var viewModels = memberships.Select(m => new UserMembershipViewModel
            {
                Id = m.Id,
                PackageName = m.MembershipPackage?.Name ?? "N/A",
                Price = m.MembershipPackage?.Price ?? 0,
                DurationDays = m.MembershipPackage?.DurationDays ?? 0,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Status = m.Status,
                CreatedAt = m.CreatedAt
            }).ToList();

            ViewBag.HasActive = await _membershipService.IsUserEligibleForRegistrationAsync(userId);

            return View(viewModels);
        }

        // GET: /Membership/Register
        public async Task<IActionResult> Register()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Đã có 1 gói được lên lịch (do xuống cấp trước đó) -> chưa cho đăng ký thêm
            var scheduled = await _membershipService.GetScheduledMembershipAsync(userId);
            if (scheduled != null)
            {
                TempData["ErrorMessage"] = $"Bạn đã có một gói tập được lên lịch chuyển sang vào ngày {scheduled.StartDate:dd/MM/yyyy}. Vui lòng chờ gói đó kích hoạt trước khi đăng ký gói khác.";
                return RedirectToAction(nameof(Index));
            }

            if (!await _membershipService.IsUserEligibleForRegistrationAsync(userId))
            {
                TempData["ErrorMessage"] = "Bạn có một gói tập đang chờ thanh toán. Vui lòng hoàn tất thanh toán hoặc hủy gói đó trước khi đăng ký gói mới.";
                return RedirectToAction(nameof(Index));
            }

            var packages = await _packageService.GetActivePackagesAsync();
            ViewBag.Packages = packages;

            // Thông tin gói đang active — để View hiển thị "còn X ngày" cho các gói xuống cấp
            var activeMembership = await _membershipService.GetActiveMembershipAsync(userId);
            ViewBag.ActiveMembership = activeMembership;

            // Nhãn Đăng ký / Gia hạn / Nâng cấp / Xuống cấp cho từng gói
            var actionLabels = new Dictionary<int, string>();
            foreach (var pkg in packages)
            {
                actionLabels[pkg.Id] = await _membershipService.GetPackageActionLabelAsync(userId, pkg.Id);
            }
            ViewBag.ActionLabels = actionLabels;

            return View();
        }

        // POST: /Membership/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int packageId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (packageId <= 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn gói tập.");
                var packages = await _packageService.GetActivePackagesAsync();
                ViewBag.Packages = packages;
                return View();
            }

            var model = new MembershipRegistrationViewModel
            {
                UserId = userId,
                MembershipPackageId = packageId
            };

            try
            {
                var result = await _membershipService.RegisterMembershipAsync(model);

                if (result.Status == "Scheduled")
                {
                    var daysRemaining = Math.Max(0, (result.StartDate.Date - DateTime.UtcNow.Date).Days);
                    var newPackage = await _packageService.GetPackageByIdAsync(packageId);
                    TempData["SuccessMessage"] =
                        $"Đã lên lịch chuyển gói thành công! Gói hiện tại của bạn còn {daysRemaining} ngày. " +
                        $"Sau đó hệ thống sẽ tự động chuyển sang gói \"{newPackage?.Name}\" (từ {result.StartDate:dd/MM/yyyy}).";
                }
                else
                {
                    TempData["SuccessMessage"] = "Đăng ký gói tập thành công!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Membership/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _membershipService.CancelMembershipAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã hủy gói tập thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy gói tập này.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Membership/Renew/5
        public async Task<IActionResult> Renew(int id)
        {
            try
            {
                var renewalInfo = await _renewalService.GetRenewalInfoAsync(id);
                return View(renewalInfo);
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Không tìm thấy gói tập.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Membership/RenewConfirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewConfirm(int id)
        {
            try
            {
                var result = await _renewalService.RenewMembershipAsync(id);
                TempData["SuccessMessage"] = "Gia hạn gói tập thành công! Ngày kết thúc mới: " +
                                             result.EndDate.ToString("dd/MM/yyyy");
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}