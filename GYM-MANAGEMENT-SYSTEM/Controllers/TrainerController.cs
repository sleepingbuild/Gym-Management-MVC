using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainerController(ITrainerService trainerService, IWebHostEnvironment webHostEnvironment)
        {
            _trainerService = trainerService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Trainer — mở cho mọi user đã đăng nhập (Member xem danh sách để đặt lịch)
        public async Task<IActionResult> Index()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            var viewModels = trainers.Select(t => new TrainerIndexViewModel
            {
                Id = t.Id,
                FullName = t.FullName,
                Specialization = t.Specialization,
                Bio = t.Bio,
                Phone = t.Phone,
                Email = t.Email,
                IsAvailable = t.IsAvailable,
                CreatedAt = t.CreatedAt,
                AvatarPath = t.AvatarPath
            }).ToList();

            return View(viewModels);
        }

        // GET: /Trainer/Details/5 — mở cho mọi user đã đăng nhập
        public async Task<IActionResult> Details(int id)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            var viewModel = new TrainerIndexViewModel
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Specialization = trainer.Specialization,
                Bio = trainer.Bio,
                Phone = trainer.Phone,
                Email = trainer.Email,
                IsAvailable = trainer.IsAvailable,
                CreatedAt = trainer.CreatedAt,
                AvatarPath = trainer.AvatarPath
            };

            return View(viewModel);
        }

        // GET: /Trainer/Create — chỉ Admin
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Trainer/Create — chỉ Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var trainer = await _trainerService.CreateTrainerAsync(model);

                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    var avatarPath = await SaveAvatarAsync(model.AvatarFile);
                    await _trainerService.UpdateAvatarAsync(trainer.Id, avatarPath);
                }

                TempData["SuccessMessage"] =
                    $"Huấn luyện viên đã được tạo thành công! Tài khoản đăng nhập: {model.Email} — " +
                    $"Mật khẩu tạm: {Services.TrainerService.DefaultTrainerPassword} (vui lòng báo HLV đổi mật khẩu sau khi đăng nhập lần đầu).";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Trainer/Edit/5 — chỉ Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            var viewModel = new TrainerEditViewModel
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Specialization = trainer.Specialization,
                Bio = trainer.Bio,
                Phone = trainer.Phone,
                Email = trainer.Email,
                DateOfBirth = trainer.DateOfBirth ?? DateTime.UtcNow.AddYears(-18),
                CurrentAvatarPath = trainer.AvatarPath,
                IsAvailable = trainer.IsAvailable
            };

            return View(viewModel);
        }

        // POST: /Trainer/Edit/5 — chỉ Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _trainerService.UpdateTrainerAsync(model);

                if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                {
                    var avatarPath = await SaveAvatarAsync(model.AvatarFile);
                    await _trainerService.UpdateAvatarAsync(model.Id, avatarPath);
                }

                TempData["SuccessMessage"] = "Huấn luyện viên đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // POST: /Trainer/Delete/5 — chỉ Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _trainerService.DeleteTrainerAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Huấn luyện viên đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa huấn luyện viên này.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Trainer/ToggleAvailability/5 — chỉ Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var result = await _trainerService.ToggleAvailabilityAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Trạng thái huấn luyện viên đã được cập nhật!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái.";
            }
            return RedirectToAction(nameof(Index));
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