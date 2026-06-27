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

        public MembershipController(
            IMembershipService membershipService,
            IMembershipPackageService packageService)
        {
            _membershipService = membershipService;
            _packageService = packageService;
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

            // Kiểm tra user đã có membership active chưa
            if (!await _membershipService.IsUserEligibleForRegistrationAsync(userId))
            {
                TempData["ErrorMessage"] = "Bạn đã có gói tập đang hoạt động. Vui lòng gia hạn hoặc hủy gói hiện tại.";
                return RedirectToAction(nameof(Index));
            }

            var packages = await _packageService.GetActivePackagesAsync();
            ViewBag.Packages = packages;
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
                await _membershipService.RegisterMembershipAsync(model);
                TempData["SuccessMessage"] = "Đăng ký gói tập thành công!";
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

        // POST: /Membership/Renew/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int id)
        {
            try
            {
                await _membershipService.RenewMembershipAsync(id);
                TempData["SuccessMessage"] = "Gia hạn gói tập thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}