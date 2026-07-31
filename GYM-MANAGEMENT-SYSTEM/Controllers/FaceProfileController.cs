using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class FaceProfileController : Controller
    {
        private readonly IFaceProfileService _faceProfileService;

        public FaceProfileController(IFaceProfileService faceProfileService)
        {
            _faceProfileService = faceProfileService;
        }

        private string? CurrentUserId =>
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.Identity?.Name;

        // ===================== Tự đăng ký (webcam, cho từng cá nhân) =====================

        // GET: /FaceProfile/Register
        public async Task<IActionResult> Register()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.HasFaceProfile = await _faceProfileService.HasFaceProfileAsync(userId);
            return View();
        }

        // POST: /FaceProfile/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] FaceProfileSaveViewModel model)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                await _faceProfileService.SaveFaceAsync(userId, model.Descriptor);
                return Json(new { success = true, message = "Đăng ký khuôn mặt thành công!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===================== Admin đăng ký hộ bằng ảnh tĩnh =====================

        // GET: /FaceProfile/AdminRegister
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminRegister()
        {
            var users = await _faceProfileService.GetEnrollableUsersAsync();
            return View(users);
        }

        // POST: /FaceProfile/AdminSave
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminSave([FromBody] AdminFaceProfileSaveViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserId))
            {
                return Json(new { success = false, message = "Vui lòng chọn thành viên cần đăng ký." });
            }

            try
            {
                await _faceProfileService.SaveFaceAsync(model.UserId, model.Descriptor);
                return Json(new { success = true, message = "Đăng ký khuôn mặt thành công!" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}