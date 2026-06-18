using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers;

public class FAQController : Controller
{
    private readonly ApplicationDbContext _context;

    public FAQController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var faqs = _context.FAQs.ToList();

        return View(faqs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(FAQ faq)
    {
        if (!ModelState.IsValid)
        {
            return View(faq);
        }

        _context.FAQs.Add(faq);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}