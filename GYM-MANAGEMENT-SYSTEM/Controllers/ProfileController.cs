using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserProfileService _profileService;

        public ProfileController(IUserProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _profileService.GetByUserIdAsync(userId);
            // profile có thể null nếu user chưa từng cập nhật hồ sơ — View xử lý empty state
            return View(profile);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _profileService.GetByUserIdAsync(userId);
            var model = profile == null
                ? new UserProfileEditViewModel()
                : new UserProfileEditViewModel
                {
                    Weight = profile.Weight,
                    Height = profile.Height,
                    Age = profile.Age,
                    Goal = profile.Goal
                };

            return View(model);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            await _profileService.CreateOrUpdateAsync(userId, model);
            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}