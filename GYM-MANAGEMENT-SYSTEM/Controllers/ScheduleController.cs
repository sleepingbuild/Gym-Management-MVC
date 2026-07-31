using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScheduleController : Controller
    {
        private readonly ITrainerScheduleService _scheduleService;
        private readonly ITrainerService _trainerService;

        public ScheduleController(
            ITrainerScheduleService scheduleService,
            ITrainerService trainerService)
        {
            _scheduleService = scheduleService;
            _trainerService = trainerService;
        }

        // GET: /Schedule?trainerId=&week=yyyy-MM-dd
        public async Task<IActionResult> Index(int? trainerId, DateOnly? week)
        {
            // "week" can be any date inside the week the admin wants to see —
            // we snap it back to that week's Monday. Omitted => current week.
            var today = DateOnly.FromDateTime(DateTime.Today);
            var refDate = week ?? today;
            int diffFromMonday = ((int)refDate.DayOfWeek + 6) % 7; // Monday = 0 ... Sunday = 6
            var weekStart = refDate.AddDays(-diffFromMonday);
            var weekEnd = weekStart.AddDays(6);

            var thisWeekDiff = ((int)today.DayOfWeek + 6) % 7;
            var thisWeekStart = today.AddDays(-thisWeekDiff);

            var schedules = await _scheduleService.GetSchedulesByWeekAsync(weekStart, weekEnd, trainerId);

            var viewModels = schedules.Select(s => new ScheduleIndexViewModel
            {
                Id = s.Id,
                TrainerId = s.TrainerId,
                TrainerName = s.Trainer?.FullName ?? "N/A",
                WorkDate = s.WorkDate,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Notes = s.Notes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            }).ToList();

            // Get trainers for filter
            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewBag.Trainers = trainers;
            ViewBag.SelectedTrainerId = trainerId;
            ViewBag.WeekStart = weekStart;
            ViewBag.WeekEnd = weekEnd;
            ViewBag.ThisWeekStart = thisWeekStart;

            return View(viewModels);
        }

        // GET: /Schedule/Create
        public async Task<IActionResult> Create()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewBag.Trainers = trainers;
            return View();
        }

        // POST: /Schedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var trainers = await _trainerService.GetAllTrainersAsync();
                ViewBag.Trainers = trainers;
                return View(model);
            }

            try
            {
                await _scheduleService.CreateScheduleAsync(model);
                TempData["SuccessMessage"] = "Lịch làm việc đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("TrainerId", ex.Message);
                var trainers = await _trainerService.GetAllTrainersAsync();
                ViewBag.Trainers = trainers;
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var trainers = await _trainerService.GetAllTrainersAsync();
                ViewBag.Trainers = trainers;
                return View(model);
            }
        }

        // GET: /Schedule/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var viewModel = new ScheduleEditViewModel
            {
                Id = schedule.Id,
                WorkDate = schedule.WorkDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Notes = schedule.Notes,
                IsActive = schedule.IsActive
            };

            return View(viewModel);
        }

        // POST: /Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ScheduleEditViewModel model)
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
                await _scheduleService.UpdateScheduleAsync(model);
                TempData["SuccessMessage"] = "Lịch làm việc đã được cập nhật thành công!";
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

        // POST: /Schedule/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Lịch làm việc đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa lịch làm việc này.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Schedule/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _scheduleService.ToggleScheduleStatusAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Trạng thái lịch đã được cập nhật!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Schedule/ByTrainer/5
        public async Task<IActionResult> ByTrainer(int trainerId)
        {
            var schedules = await _scheduleService.GetSchedulesByTrainerIdAsync(trainerId);
            var viewModels = schedules.Select(s => new ScheduleIndexViewModel
            {
                Id = s.Id,
                TrainerId = s.TrainerId,
                TrainerName = s.Trainer?.FullName ?? "N/A",
                WorkDate = s.WorkDate,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Notes = s.Notes,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            }).ToList();

            return Json(viewModels);
        }
    }
}