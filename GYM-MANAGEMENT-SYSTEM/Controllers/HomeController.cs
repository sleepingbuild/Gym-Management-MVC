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