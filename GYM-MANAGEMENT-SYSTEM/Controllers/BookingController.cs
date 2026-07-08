using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly ITrainerService _trainerService;

        public BookingController(
            IBookingService bookingService,
            ITrainerService trainerService)
        {
            _bookingService = bookingService;
            _trainerService = trainerService;
        }

        // GET: /Booking
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            var viewModels = bookings.Select(b => new BookingIndexViewModel
            {
                Id = b.Id,
                UserId = b.UserId,
                TrainerId = b.TrainerId,
                TrainerName = b.Trainer?.FullName ?? "N/A",
                SessionDate = b.SessionDate,
                TimeSlot = b.TimeSlot,
                Status = b.Status,
                Notes = b.Notes,
                CreatedAt = b.CreatedAt
            }).ToList();

            return View(viewModels);
        }

        // GET: /Booking/Create
        public async Task<IActionResult> Create()
        {
            var trainers = await _trainerService.GetAvailableTrainersAsync();
            ViewBag.Trainers = trainers;

            // Set default date to tomorrow
            var model = new BookingCreateViewModel
            {
                SessionDate = DateTime.UtcNow.AddDays(1).Date
            };

            return View(model);
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var trainers = await _trainerService.GetAvailableTrainersAsync();
                ViewBag.Trainers = trainers;
                return View(model);
            }

            // Get current user ID
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            model.UserId = userId;

            try
            {
                var booking = await _bookingService.CreateBookingAsync(model);
                TempData["SuccessMessage"] = "Đặt lịch thành công! Vui lòng chờ xác nhận từ huấn luyện viên.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("TrainerId", ex.Message);
                var trainers = await _trainerService.GetAvailableTrainersAsync();
                ViewBag.Trainers = trainers;
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var trainers = await _trainerService.GetAvailableTrainersAsync();
                ViewBag.Trainers = trainers;
                return View(model);
            }
        }

        // POST: /Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Đã hủy đặt lịch thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy đặt lịch này.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Booking/Upcoming
        public async Task<IActionResult> Upcoming()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = await _bookingService.GetUpcomingBookingsAsync(userId);
            var viewModels = bookings.Select(b => new BookingIndexViewModel
            {
                Id = b.Id,
                UserId = b.UserId,
                TrainerId = b.TrainerId,
                TrainerName = b.Trainer?.FullName ?? "N/A",
                SessionDate = b.SessionDate,
                TimeSlot = b.TimeSlot,
                Status = b.Status,
                Notes = b.Notes,
                CreatedAt = b.CreatedAt
            }).ToList();

            return View(viewModels);
        }

        // GET: /Booking/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var viewModel = new BookingIndexViewModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TrainerId = booking.TrainerId,
                TrainerName = booking.Trainer?.FullName ?? "N/A",
                SessionDate = booking.SessionDate,
                TimeSlot = booking.TimeSlot,
                Status = booking.Status,
                Notes = booking.Notes,
                CreatedAt = booking.CreatedAt
            };

            return View(viewModel);
        }

        // GET: /Booking/Calendar
        public async Task<IActionResult> Calendar()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            ViewBag.Trainers = trainers;
            return View();
        }

        // API: /Booking/GetSlots
        [HttpGet]
        public async Task<IActionResult> GetSlots(int trainerId, DateTime date)
        {
            var slots = await _bookingService.GetBookingsByDateAsync(date);
            var trainerSlots = slots.Where(b => b.TrainerId == trainerId && b.Status != "Cancelled")
                                    .Select(b => b.TimeSlot)
                                    .ToList();

            return Json(new { bookedSlots = trainerSlots });
        }

        // GET: /Booking/History
        public async Task<IActionResult> History(BookingHistoryFilterViewModel filter)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy dữ liệu với filter
            IEnumerable<Booking> bookings;

            if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "Tất cả")
            {
                bookings = await _bookingService.GetBookingHistoryByStatusAsync(userId, filter.Status);
            }
            else if (!string.IsNullOrEmpty(filter.SearchTerm) || filter.FromDate.HasValue || filter.ToDate.HasValue)
            {
                bookings = await _bookingService.SearchBookingsAsync(userId, filter.SearchTerm, filter.FromDate, filter.ToDate);
            }
            else
            {
                bookings = await _bookingService.GetBookingHistoryAsync(userId);
            }

            var viewModels = bookings.Select(b => new BookingIndexViewModel
            {
                Id = b.Id,
                UserId = b.UserId,
                TrainerId = b.TrainerId,
                TrainerName = b.Trainer?.FullName ?? "N/A",
                SessionDate = b.SessionDate,
                TimeSlot = b.TimeSlot,
                Status = b.Status,
                Notes = b.Notes,
                CreatedAt = b.CreatedAt
            }).ToList();

            // Lấy thống kê
            var stats = await _bookingService.GetBookingStatisticsAsync(userId);
            ViewBag.Statistics = stats;

            return View(viewModels);
        }

        // GET: /Booking/Statistics
        public async Task<IActionResult> Statistics()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var stats = await _bookingService.GetBookingStatisticsAsync(userId);
            return View(stats);
        }

        // GET: /Booking/Export
        public async Task<IActionResult> Export(DateTime? fromDate, DateTime? toDate)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var bookings = await _bookingService.GetBookingHistoryAsync(userId, fromDate, toDate);

            // Tạo file CSV
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Ngày,Giờ,HLV,Trạng thái,Ghi chú");

            foreach (var b in bookings)
            {
                csv.AppendLine($"{b.SessionDate:dd/MM/yyyy},{b.SessionDate:HH:mm},{b.Trainer?.FullName},{b.Status},{b.Notes}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"booking_history_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}