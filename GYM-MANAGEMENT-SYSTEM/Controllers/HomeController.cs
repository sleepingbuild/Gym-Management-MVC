using GYM_MANAGEMENT_SYSTEM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Admin không dùng trang chủ chung của Member/Trainer nữa —
            // vào thẳng Dashboard quản trị (đã có sẵn từ DashboardController).
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Trainer vào thẳng Bảng điều khiển HLV, không dùng trang chủ chung Member nữa.
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Trainer"))
            {
                return RedirectToAction("Index", "TrainerPortal");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        // Ví dụ: Lấy danh sách packages
        public async Task<IActionResult> Packages()
        {
            var packages = await _context.MembershipPackages
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();
            return View(packages);
        }
    }
}