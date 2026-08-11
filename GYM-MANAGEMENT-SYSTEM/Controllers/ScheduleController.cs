using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScheduleController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly ITrainerService _trainerService;

        public ScheduleController(
            IBookingService bookingService,
            ITrainerService trainerService)
        {
            _bookingService = bookingService;
            _trainerService = trainerService;
        }

        // GET: /Schedule?trainerId=&week=yyyy-MM-dd
        public async Task<IActionResult> Index(int? trainerId, DateOnly? week)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var refDate = week ?? today;
            int diffFromMonday = ((int)refDate.DayOfWeek + 6) % 7; // Monday = 0 ... Sunday = 6
            var weekStart = refDate.AddDays(-diffFromMonday);
            var weekEnd = weekStart.AddDays(6);

            var thisWeekDiff = ((int)today.DayOfWeek + 6) % 7;
            var thisWeekStart = today.AddDays(-thisWeekDiff);

            var bookings = (await _bookingService.GetBookingsByDateRangeAsync(weekStart, weekEnd, trainerId)).ToList();

            var memberInfo = await _bookingService.GetMemberDisplayInfoAsync(bookings.Select(b => b.UserId).Distinct());

            var viewModels = bookings.Select(b =>
            {
                memberInfo.TryGetValue(b.UserId, out var info);
                var (start, end) = ParseTimeSlot(b.TimeSlot);

                return new ScheduleBookingViewModel
                {
                    Id = b.Id,
                    TrainerId = b.TrainerId,
                    TrainerName = b.Trainer?.FullName ?? "N/A",
                    MemberName = info.FullName ?? b.UserId,
                    WorkDate = DateOnly.FromDateTime(b.SessionDate),
                    StartTime = start,
                    EndTime = end,
                    Status = b.Status,
                    Notes = b.Notes
                };
            }).ToList();

            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewBag.Trainers = trainers;
            ViewBag.SelectedTrainerId = trainerId;
            ViewBag.WeekStart = weekStart;
            ViewBag.WeekEnd = weekEnd;
            ViewBag.ThisWeekStart = thisWeekStart;
            ViewBag.WorkingHourStart = BookingService.WorkingHourStart;
            ViewBag.WorkingHourEnd = BookingService.WorkingHourEnd;

            return View(viewModels);
        }

        // GET: /Schedule/Create
       
        // Booking tạo ra sẽ ở trạng thái Confirmed ngay, không cần Trainer xác nhận.
        public async Task<IActionResult> Create()
        {
            await PopulateCreateFormData();
            var model = new AdminBookingCreateViewModel
            {
                SessionDate = DateTime.Today.AddDays(1)
            };
            return View(model);
        }

        // POST: /Schedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminBookingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateFormData();
                return View(model);
            }

            try
            {
                await _bookingService.CreateBookingByAdminAsync(model);
                TempData["SuccessMessage"] = "Đã đặt lịch cho thành viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PopulateCreateFormData();
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PopulateCreateFormData();
                return View(model);
            }
        }

        // POST: /Schedule/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _bookingService.ConfirmBookingAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Đã xác nhận buổi tập!" : "Không thể xác nhận buổi tập này.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Schedule/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await _bookingService.CompleteBookingAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Đã đánh dấu buổi tập hoàn thành!" : "Không thể cập nhật buổi tập này.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Schedule/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Đã hủy buổi tập!" : "Không thể hủy buổi tập này.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetTrainerSlots(int trainerId)
        {
            var slots = await _bookingService.GetTimeSlotsForTrainerAsync(trainerId);
            return Json(slots);
        }

        private async Task PopulateCreateFormData()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            var members = await _bookingService.GetBookableMembersAsync();

            ViewBag.Trainers = trainers;
            ViewBag.Members = members;
            ViewBag.TimeSlots = _bookingService.GetFixedTimeSlots();
        }

        private static (TimeOnly Start, TimeOnly End) ParseTimeSlot(string timeSlot)
        {
            var parts = timeSlot.Split('-');

            if (parts.Length == 2 &&
                TimeOnly.TryParse(parts[0], out var rangeStart) &&
                TimeOnly.TryParse(parts[1], out var rangeEnd))
            {
                return (rangeStart, rangeEnd);
            }

            if (parts.Length == 1 && TimeOnly.TryParse(parts[0], out var singleStart))
            {
                return (singleStart, singleStart.AddHours(1));
            }

            return (TimeOnly.MinValue, TimeOnly.MinValue);
        }
    }
}