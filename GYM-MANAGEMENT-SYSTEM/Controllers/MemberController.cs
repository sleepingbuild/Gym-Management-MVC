using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    // Quản lý tài khoản Member — KHÔNG có action Create vì member tự đăng ký
    // qua /Account/Register, Admin chỉ xem/sửa/khóa/xóa.
    [Authorize(Roles = "Admin")]
    public class MemberController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMembershipService _membershipService;

        public MemberController(
            UserManager<ApplicationUser> userManager,
            IMembershipService membershipService)
        {
            _userManager = userManager;
            _membershipService = membershipService;
        }

        // GET: /Member
        public async Task<IActionResult> Index()
        {
            var members = await _userManager.GetUsersInRoleAsync("Member");
            var list = new List<MemberIndexViewModel>();

            foreach (var user in members)
            {
                var memberships = await _membershipService.GetUserMembershipsAsync(user.Id);
                var activeMembership = memberships.FirstOrDefault(m => m.Status == "Active");

                list.Add(new MemberIndexViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "N/A",
                    PhoneNumber = user.PhoneNumber ?? "",
                    CreatedAt = user.CreatedAt,
                    IsLockedOut = await _userManager.IsLockedOutAsync(user),
                    ActivePackageName = activeMembership?.MembershipPackage?.Name
                });
            }

            return View(list.OrderByDescending(m => m.CreatedAt).ToList());
        }

        // GET: /Member/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new MemberEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // POST: /Member/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MemberEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra email trùng với tài khoản khác
            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _userManager.FindByEmailAsync(model.Email);
                if (existing != null && existing.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email này đã được sử dụng bởi tài khoản khác.");
                    return View(model);
                }

                await _userManager.SetEmailAsync(user, model.Email);
                await _userManager.SetUserNameAsync(user, model.Email);
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Đã cập nhật thông tin thành viên!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Member/ToggleLock/{id} — khóa = không thể đăng nhập, mở khóa = đăng nhập lại bình thường
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var isCurrentlyLockedOut = await _userManager.IsLockedOutAsync(user);

            if (isCurrentlyLockedOut)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["SuccessMessage"] = "Đã mở khóa tài khoản. Thành viên có thể đăng nhập lại.";
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                // Khóa vô thời hạn cho tới khi Admin chủ động mở khóa lại
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["SuccessMessage"] = "Đã khóa tài khoản. Thành viên sẽ không thể đăng nhập.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Member/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Đã xóa tài khoản thành viên.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa tài khoản này.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể xóa — tài khoản này còn dữ liệu liên quan (đặt lịch, thanh toán...).";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}