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

        // POST: /FaceAttendance/CheckIn
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckIn([FromBody] FaceScanRequestViewModel model)
        {
            if (model?.Descriptor == null || model.Descriptor.Length == 0)
            {
                return Json(new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = "Không nhận được dữ liệu khuôn mặt."
                });
            }

            var matchedUserId = await _faceProfileService.FindMatchingUserIdAsync(model.Descriptor);
            if (matchedUserId == null)
            {
                return Json(new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = "Không nhận dạng được khuôn mặt — chưa đăng ký hoặc không đủ rõ."
                });
            }

            var result = await _faceAttendanceService.ProcessScanAsync(matchedUserId);
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

        // POST: /FaceAttendance/SelfCheckInSubmit
        [HttpPost]
        [Authorize(Roles = "Trainer,Member")]
        public async Task<IActionResult> SelfCheckInSubmit([FromBody] FaceScanRequestViewModel model)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (model?.Descriptor == null || model.Descriptor.Length == 0)
            {
                return Json(new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = "Không nhận được dữ liệu khuôn mặt."
                });
            }

            var isMatch = await _faceProfileService.VerifyOwnFaceAsync(userId, model.Descriptor);
            if (!isMatch)
            {
                return Json(new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = "Khuôn mặt không khớp với tài khoản đang đăng nhập. Vui lòng thử lại hoặc liên hệ Admin nếu bạn nghĩ đây là nhầm lẫn."
                });
            }

            var result = await _faceAttendanceService.ProcessScanAsync(userId);
            return Json(result);
        }
    }
}