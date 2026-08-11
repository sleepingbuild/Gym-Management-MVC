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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMembershipService _membershipService;
        private readonly IPaymentService _paymentService;
        private readonly ITrainerService _trainerService;

        public ProfileController(
            IUserProfileService profileService,
            IWebHostEnvironment webHostEnvironment,
            IMembershipService membershipService,
            IPaymentService paymentService,
            ITrainerService trainerService)
        {
            _profileService = profileService;
            _webHostEnvironment = webHostEnvironment;
            _membershipService = membershipService;
            _paymentService = paymentService;
            _trainerService = trainerService;
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

            // Trainer xem thông tin nghề nghiệp của chính mình — đọc thẳng từ bảng
            // Trainer (cùng bảng Admin quản lý), không phải UserProfile (chỉ số cơ thể).
            if (User.IsInRole("Trainer"))
            {
                var trainer = await _trainerService.GetTrainerByUserIdAsync(userId);
                return View("TrainerIndex", trainer);
            }

            var profile = await _profileService.GetByUserIdAsync(userId);

            return View(profile);
        }

        // GET: /Profile/Memberships — "Thẻ tập": chỉ hiện gói ĐANG DÙNG hiện tại
        // (xác định theo thanh toán thành công gần nhất), không phải lịch sử.
        public async Task<IActionResult> Memberships()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var current = await _paymentService.GetCurrentMembershipAsync(userId);

            MyMembershipViewModel? viewModel = null;
            if (current != null)
            {
                viewModel = new MyMembershipViewModel
                {
                    Id = current.Id,
                    PackageName = current.MembershipPackage?.Name ?? "N/A",
                    PackageDescription = current.MembershipPackage?.Description ?? string.Empty,
                    Price = current.MembershipPackage?.Price ?? 0,
                    DurationDays = current.MembershipPackage?.DurationDays ?? 0,
                    StartDate = current.StartDate,
                    EndDate = current.EndDate,
                    Status = current.Status
                };
            }

            return View(viewModel);
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

            if (User.IsInRole("Trainer"))
            {
                var trainer = await _trainerService.GetTrainerByUserIdAsync(userId);
                if (trainer == null)
                {
                    return NotFound();
                }

                var trainerModel = new TrainerProfileEditViewModel
                {
                    TrainerId = trainer.Id,
                    FullName = trainer.FullName,
                    Specialization = trainer.Specialization,
                    Bio = trainer.Bio,
                    Phone = trainer.Phone,
                    Email = trainer.Email,
                    IsAvailable = trainer.IsAvailable,
                    CurrentAvatarPath = trainer.AvatarPath
                };

                return View("EditTrainerProfile", trainerModel);
            }

            var profile = await _profileService.GetByUserIdAsync(userId);
            var model = profile == null
                ? new UserProfileEditViewModel()
                : new UserProfileEditViewModel
                {
                    Weight = profile.Weight,
                    Height = profile.Height,
                    Age = profile.Age,
                    Goal = profile.Goal,
                    CurrentAvatarPath = profile.AvatarPath
                };

            return View(model);
        }

        // POST: /Profile/EditTrainerProfile — Trainer tự sửa hồ sơ nghề nghiệp của mình
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainerProfile(TrainerProfileEditViewModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Đảm bảo Trainer chỉ sửa được đúng hồ sơ của chính mình
            var ownTrainer = await _trainerService.GetTrainerByUserIdAsync(userId);
            if (ownTrainer == null || ownTrainer.Id != model.TrainerId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                model.Email = ownTrainer.Email;
                model.IsAvailable = ownTrainer.IsAvailable;
                model.CurrentAvatarPath = ownTrainer.AvatarPath;
                return View("EditTrainerProfile", model);
            }

            try
            {
                await _trainerService.UpdateOwnProfileAsync(model.TrainerId, model);

                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    var avatarPath = await SaveAvatarAsync(model.AvatarFile);
                    await _trainerService.UpdateAvatarAsync(model.TrainerId, avatarPath);
                }

                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is KeyNotFoundException)
            {
                ModelState.AddModelError("", ex.Message);
                model.Email = ownTrainer.Email;
                model.IsAvailable = ownTrainer.IsAvailable;
                model.CurrentAvatarPath = ownTrainer.AvatarPath;
                return View("EditTrainerProfile", model);
            }
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

            try
            {
                await _profileService.CreateOrUpdateAsync(userId, model);

                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    var avatarPath = await SaveAvatarAsync(model.AvatarFile);
                    await _profileService.UpdateAvatarAsync(userId, avatarPath);
                }

                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        private async Task<string> SaveAvatarAsync(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
            {
                throw new InvalidOperationException("Chỉ chấp nhận ảnh định dạng JPG, PNG hoặc WEBP.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("Kích thước ảnh không được vượt quá 5MB.");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/avatars/{fileName}";
        }
    }
}