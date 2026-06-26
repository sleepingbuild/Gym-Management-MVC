using GYM_MANAGEMENT_SYSTEM.AI.Services;
using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers;

public class FAQController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly DatasetImportService _importService;
    public FAQController(
    ApplicationDbContext context,
    DatasetImportService importService)
    {
        _context = context;
        _importService = importService;
    }

    public IActionResult ImportTest()
    {
        string path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "AI",
            "Datasets",
            "test_faq.json");

        int count = _importService.ImportFAQs(path);

        return Content($"Imported {count} FAQs");
    }

    public IActionResult Index(string search, string category)
    {
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

        return View(faqs.ToList());
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

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var faq = _context.FAQs.Find(id);

        if (faq == null)
        {
            return NotFound();
        }

        return View(faq);
    }

    [HttpPost]
    public IActionResult Edit(FAQ faq)
    {
        if (!ModelState.IsValid)
        {
            return View(faq);
        }

        _context.FAQs.Update(faq);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var faq = _context.FAQs.Find(id);

        if (faq == null)
        {
            return NotFound();
        }

        _context.FAQs.Remove(faq);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}