using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(
    string search,
    string category,
    int page = 1)
        {
            int pageSize = 10;

            ViewBag.Search = search;
            ViewBag.Category = category;

            var faqs = _context.FAQs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                faqs = faqs.Where(x =>
                    x.Question.Contains(search) ||
                    x.Answer.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                faqs = faqs.Where(x =>
                    x.Category == category);
            }

            ViewBag.Categories = _context.FAQs
                .Select(x => x.Category)
                .Distinct()
                .ToList();

            int totalItems = faqs.Count();

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(totalItems / (double)pageSize);

            var result = faqs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
