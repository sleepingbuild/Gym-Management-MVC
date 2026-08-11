using GYM_MANAGEMENT_SYSTEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AttendanceController : Controller
    {
        private readonly ITrainerAttendanceService _attendanceService;

        public AttendanceController(ITrainerAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }


        public async Task<IActionResult> Index(DateTime? date)
        {
            var target = date ?? DateTime.Now.Date;
            var report = await _attendanceService.GetDailyReportAsync(target);
            return View(report);
        }
    }
}