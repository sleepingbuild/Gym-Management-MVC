using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class WorkoutController : Controller
    {
        private readonly IWorkoutProgressService _workoutService;

        public WorkoutController(IWorkoutProgressService workoutService)
        {
            _workoutService = workoutService;
        }

        // GET: /Workout
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var progress = await _workoutService.GetUserProgressAsync(userId);
            var viewModels = progress.Select(p => new WorkoutIndexViewModel
            {
                Id = p.Id,
                RecordedAt = p.RecordedAt,
                Weight = p.Weight,
                Height = p.Height,
                BodyFatPercentage = p.BodyFatPercentage,
                MuscleMass = p.MuscleMass,
                WaistCircumference = p.WaistCircumference,
                Notes = p.Notes
            }).ToList();

            var stats = await _workoutService.GetStatisticsAsync(userId);
            ViewBag.Statistics = stats;

            return View(viewModels);
        }

        // GET: /Workout/Create
        public IActionResult Create()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new WorkoutCreateViewModel
            {
                UserId = userId
            };

            return View(model);
        }

        // POST: /Workout/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkoutCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _workoutService.CreateProgressAsync(model);
                TempData["SuccessMessage"] = "Đã lưu tiến trình tập luyện!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Workout/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var progress = await _workoutService.GetProgressByIdAsync(id);
            if (progress == null)
            {
                return NotFound();
            }

            var viewModel = new WorkoutEditViewModel
            {
                Id = progress.Id,
                UserId = progress.UserId,
                Weight = progress.Weight,
                Height = progress.Height,
                BodyFatPercentage = progress.BodyFatPercentage,
                MuscleMass = progress.MuscleMass,
                WaistCircumference = progress.WaistCircumference,
                Notes = progress.Notes
            };

            return View(viewModel);
        }

        // POST: /Workout/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkoutEditViewModel model)
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
                await _workoutService.UpdateProgressAsync(model);
                TempData["SuccessMessage"] = "Đã cập nhật tiến trình!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // POST: /Workout/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _workoutService.DeleteProgressAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã xóa tiến trình!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa tiến trình.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Workout/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var progress = await _workoutService.GetProgressByIdAsync(id);
            if (progress == null)
            {
                return NotFound();
            }

            var viewModel = new WorkoutIndexViewModel
            {
                Id = progress.Id,
                RecordedAt = progress.RecordedAt,
                Weight = progress.Weight,
                Height = progress.Height,
                BodyFatPercentage = progress.BodyFatPercentage,
                MuscleMass = progress.MuscleMass,
                WaistCircumference = progress.WaistCircumference,
                Notes = progress.Notes
            };

            return View(viewModel);
        }

        // GET: /Workout/Statistics
        public async Task<IActionResult> Statistics()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var stats = await _workoutService.GetStatisticsAsync(userId);
            return View(stats);
        }

        // GET: /Workout/Chart
        public async Task<IActionResult> Chart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var data = await _workoutService.GetUserProgressAsync(userId);
            var chartData = data.OrderBy(w => w.RecordedAt).Select(w => new
            {
                date = w.RecordedAt.ToString("dd/MM"),
                weight = w.Weight,
                bodyFat = w.BodyFatPercentage,
                muscleMass = w.MuscleMass
            }).ToList();

            return Json(chartData);
        }
    }
}