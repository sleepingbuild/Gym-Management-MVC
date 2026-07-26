using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class TrainerPortalController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IBookingService _bookingService;
        private readonly IUserProfileService _profileService;
        private readonly IWorkoutProgressService _workoutProgressService;
        private readonly ITrainerAttendanceService _attendanceService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainerPortalController(
            ITrainerService trainerService,
            IBookingService bookingService,
            IUserProfileService profileService,
            IWorkoutProgressService workoutProgressService,
            ITrainerAttendanceService attendanceService,
            UserManager<ApplicationUser> userManager)
        {
            _trainerService = trainerService;
            _bookingService = bookingService;
            _profileService = profileService;
            _workoutProgressService = workoutProgressService;
            _attendanceService = attendanceService;
            _userManager = userManager;
        }

        // Lấy bản ghi Trainer ứng với tài khoản đang đăng nhập
        private async Task<Trainer?> GetCurrentTrainerAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return await _trainerService.GetTrainerByUserIdAsync(userId);
        }

        // GET: /TrainerPortal
        public async Task<IActionResult> Index()
        {
            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ huấn luyện viên nào. Vui lòng liên hệ Admin.";
                return View(new TrainerDashboardViewModel());
            }

            var bookings = (await _bookingService.GetTrainerBookingsAsync(trainer.Id)).ToList();
            var today = DateTime.Today;

            var todaySessions = new List<TrainerBookingViewModel>();
            foreach (var b in bookings.Where(b => b.SessionDate.Date == today).OrderBy(b => b.TimeSlot))
            {
                var user = await _userManager.FindByIdAsync(b.UserId);
                todaySessions.Add(new TrainerBookingViewModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    StudentName = user?.FullName ?? "N/A",
                    SessionDate = b.SessionDate,
                    TimeSlot = b.TimeSlot,
                    Status = b.Status,
                    Notes = b.Notes
                });
            }

            var model = new TrainerDashboardViewModel
            {
                TrainerName = trainer.FullName,
                TotalStudents = bookings.Select(b => b.UserId).Distinct().Count(),
                TodaySessionsCount = bookings.Count(b => b.SessionDate.Date == today),
                UpcomingSessionsCount = bookings.Count(b => b.SessionDate.Date > today && b.Status != "Cancelled"),
                PendingConfirmCount = bookings.Count(b => b.Status == "Pending"),
                TodaySessions = todaySessions
            };

            return View(model);
        }

        // GET: /TrainerPortal/Students — danh sách khách hàng đang tập với trainer này
        public async Task<IActionResult> Students()
        {
            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ huấn luyện viên nào.";
                return View(new List<TrainerStudentViewModel>());
            }

            var bookings = await _bookingService.GetTrainerBookingsAsync(trainer.Id);
            var grouped = bookings.GroupBy(b => b.UserId);

            var students = new List<TrainerStudentViewModel>();
            foreach (var g in grouped)
            {
                var user = await _userManager.FindByIdAsync(g.Key);
                var profile = await _profileService.GetByUserIdAsync(g.Key);

                students.Add(new TrainerStudentViewModel
                {
                    UserId = g.Key,
                    FullName = user?.FullName ?? "N/A",
                    Email = user?.Email ?? "N/A",
                    Age = profile?.Age,
                    Weight = profile?.Weight,
                    Height = profile?.Height,
                    Goal = profile?.Goal,
                    TotalSessions = g.Count(),
                    LastSessionDate = g.Max(b => b.SessionDate)
                });
            }

            return View(students.OrderByDescending(s => s.LastSessionDate).ToList());
        }

        // GET: /TrainerPortal/Timetable — thời khoá biểu đầy đủ
        public async Task<IActionResult> Timetable()
        {
            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ huấn luyện viên nào.";
                return View(new List<TrainerBookingViewModel>());
            }

            var bookings = await _bookingService.GetTrainerBookingsAsync(trainer.Id);
            var list = new List<TrainerBookingViewModel>();

            foreach (var b in bookings.OrderBy(b => b.SessionDate).ThenBy(b => b.TimeSlot))
            {
                var user = await _userManager.FindByIdAsync(b.UserId);
                list.Add(new TrainerBookingViewModel
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    StudentName = user?.FullName ?? "N/A",
                    SessionDate = b.SessionDate,
                    TimeSlot = b.TimeSlot,
                    Status = b.Status,
                    Notes = b.Notes
                });
            }

            return View(list);
        }

        // GET: /TrainerPortal/BookingDetail/5 — chi tiết 1 buổi tập
        public async Task<IActionResult> BookingDetail(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null || booking.TrainerId != trainer.Id)
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(booking.UserId);
            var profile = await _profileService.GetByUserIdAsync(booking.UserId);

            var model = new TrainerBookingDetailViewModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TrainerId = booking.TrainerId,
                StudentName = user?.FullName ?? "N/A",
                StudentEmail = user?.Email ?? "N/A",
                StudentAge = profile?.Age,
                StudentWeight = profile?.Weight,
                StudentHeight = profile?.Height,
                StudentGoal = profile?.Goal,
                SessionDate = booking.SessionDate,
                TimeSlot = booking.TimeSlot,
                Status = booking.Status,
                Notes = booking.Notes
            };

            return View(model);
        }

        // POST: /TrainerPortal/SaveNote — trainer ghi chú nội dung buổi tập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNote(int id, string notes)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null || booking.TrainerId != trainer.Id)
            {
                return Forbid();
            }

            var editModel = new BookingEditViewModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TrainerId = booking.TrainerId,
                SessionDate = booking.SessionDate,
                TimeSlot = booking.TimeSlot,
                Notes = notes ?? string.Empty,
                Status = booking.Status
            };

            await _bookingService.UpdateBookingAsync(editModel);
            TempData["SuccessMessage"] = "Đã lưu ghi chú buổi tập!";
            return RedirectToAction(nameof(BookingDetail), new { id });
        }

        // POST: /TrainerPortal/MarkCompleted/5 — xác nhận hội viên đã tập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null || booking.TrainerId != trainer.Id)
            {
                return Forbid();
            }

            await _bookingService.CompleteBookingAsync(id);
            TempData["SuccessMessage"] = "Đã xác nhận hội viên đã tập buổi này!";
            return RedirectToAction(nameof(BookingDetail), new { id });
        }

        // GET: /TrainerPortal/Progress — tiến trình gần nhất của tất cả hội viên đang dạy
        public async Task<IActionResult> Progress()
        {
            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ huấn luyện viên nào.";
                return View(new List<TrainerStudentProgressViewModel>());
            }

            var bookings = await _bookingService.GetTrainerBookingsAsync(trainer.Id);
            var studentIds = bookings.Select(b => b.UserId).Distinct();

            var result = new List<TrainerStudentProgressViewModel>();
            foreach (var userId in studentIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var latest = await _workoutProgressService.GetLatestAsync(userId);

                var item = new TrainerStudentProgressViewModel
                {
                    UserId = userId,
                    FullName = user?.FullName ?? "N/A",
                    Email = user?.Email ?? "N/A",
                    HasRecords = latest != null
                };

                if (latest != null)
                {
                    item.LatestWeight = latest.Weight;
                    item.LatestHeight = latest.Height;
                    item.LatestBMI = latest.BMI;
                    item.LatestBMICategory = latest.BMICategory;
                    item.LatestBMIStatus = latest.BMIStatus;
                    item.LatestBodyFatPercentage = latest.BodyFatPercentage;
                    item.LatestMuscleMass = latest.MuscleMass;
                    item.RecordedAt = latest.RecordedAt;
                }

                result.Add(item);
            }

            // Hội viên có dữ liệu mới nhất lên trước; hội viên chưa ghi nhận gì xuống cuối
            var ordered = result
                .OrderByDescending(r => r.HasRecords)
                .ThenByDescending(r => r.RecordedAt)
                .ToList();

            return View(ordered);
        }

        // GET: /TrainerPortal/ProgressHistory?userId=... — lịch sử đầy đủ của 1 hội viên
        public async Task<IActionResult> ProgressHistory(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null)
            {
                return Forbid();
            }

            // Bảo mật: chỉ cho xem nếu hội viên này thực sự có đặt lịch với trainer đang đăng nhập
            var bookings = await _bookingService.GetTrainerBookingsAsync(trainer.Id);
            var isMyStudent = bookings.Any(b => b.UserId == userId);
            if (!isMyStudent)
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);
            ViewBag.StudentName = user?.FullName ?? "N/A";
            ViewBag.StudentEmail = user?.Email ?? "N/A";

            var history = (await _workoutProgressService.GetUserProgressAsync(userId))
                .OrderByDescending(w => w.RecordedAt)
                .ToList();

            return View(history);
        }

        // GET: /TrainerPortal/Attendance — trạng thái chấm công hôm nay + lịch sử
        public async Task<IActionResult> Attendance()
        {
            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ huấn luyện viên nào.";
                return View(new TrainerAttendanceStatusViewModel());
            }

            var status = await _attendanceService.GetStatusAsync(trainer.Id);
            return View(status);
        }

        // POST: /TrainerPortal/CheckIn — trainer xác nhận đã đến dạy hôm nay
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(string? notes)
        {
            var trainer = await GetCurrentTrainerAsync();
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn chưa được liên kết với hồ sơ huấn luyện viên nào.";
                return RedirectToAction(nameof(Attendance));
            }

            try
            {
                await _attendanceService.CheckInAsync(trainer.Id, notes);
                TempData["SuccessMessage"] = "Đã chấm công thành công cho hôm nay!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Attendance));
        }
    }
}