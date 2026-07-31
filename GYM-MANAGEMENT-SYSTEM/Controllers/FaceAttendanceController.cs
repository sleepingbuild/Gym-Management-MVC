using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class FaceAttendanceController : Controller
    {
        private readonly IFaceProfileService _faceProfileService;
        private readonly IFaceAttendanceService _faceAttendanceService;

        public FaceAttendanceController(
            IFaceProfileService faceProfileService,
            IFaceAttendanceService faceAttendanceService)
        {
            _faceProfileService = faceProfileService;
            _faceAttendanceService = faceAttendanceService;
        }

        private string? CurrentUserId =>
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.Identity?.Name;

        // ===================== Kiosk (Admin) — so khớp với TẤT CẢ hồ sơ =====================

        // GET: /FaceAttendance/Kiosk
        [Authorize(Roles = "Admin")]
        public IActionResult Kiosk()
        {
            return View();
        }

        // GET: /FaceAttendance/GetProfiles
        // Trả về toàn bộ descriptor để Kiosk so khớp trực tiếp (client-side). Chỉ Admin được gọi.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _faceProfileService.GetAllForKioskAsync();
            return Json(profiles);
        }

        // POST: /FaceAttendance/CheckIn
        // Kiosk đã tự so khớp xong, chỉ gửi userId khớp được lên để ghi nhận điểm danh.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckIn([FromBody] FaceCheckInRequestViewModel model)
        {
            if (string.IsNullOrEmpty(model?.UserId))
            {
                return Json(new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = "Thiếu thông tin người dùng."
                });
            }

            var result = await _faceAttendanceService.CheckInAsync(model.UserId);
            return Json(result);
        }

        // ===================== Tự điểm danh (Trainer/Member) — chỉ so khớp với CHÍNH HỌ =====================

        // GET: /FaceAttendance/SelfCheckIn
        [Authorize(Roles = "Trainer,Member")]
        public async Task<IActionResult> SelfCheckIn()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.HasFaceProfile = await _faceProfileService.HasFaceProfileAsync(userId);
            return View();
        }

        // GET: /FaceAttendance/GetMyDescriptor
        // Chỉ trả về descriptor của CHÍNH người đang đăng nhập — không lộ dữ liệu người khác.
        [HttpGet]
        [Authorize(Roles = "Trainer,Member")]
        public async Task<IActionResult> GetMyDescriptor()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var descriptor = await _faceProfileService.GetDescriptorAsync(userId);
            if (descriptor == null)
            {
                return NotFound(new { message = "Bạn chưa được đăng ký khuôn mặt. Vui lòng liên hệ Admin." });
            }

            return Json(new { descriptor });
        }

        // POST: /FaceAttendance/SelfCheckInSubmit
        // Không nhận userId từ client — luôn dùng userId của người đang đăng nhập,
        // tránh trường hợp Trainer gửi userId của người khác lên để điểm danh hộ.
        [HttpPost]
        [Authorize(Roles = "Trainer,Member")]
        public async Task<IActionResult> SelfCheckInSubmit()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _faceAttendanceService.CheckInAsync(userId);
            return Json(result);
        }
    }
}