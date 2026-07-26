using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Controllers;

public class FeedbackController : Controller
{
    private readonly ApplicationDbContext _context;

    public FeedbackController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RateAnswer([FromForm] int chatHistoryId, [FromForm] int rating, [FromForm] string? comment)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "guest";

        var existing = _context.Feedbacks.FirstOrDefault(f =>
            f.UserId == userId && f.ChatHistoryId == chatHistoryId && f.Type == "AnswerRating");

        if (existing != null)
        {
            existing.Rating = rating;
            if (!string.IsNullOrWhiteSpace(comment)) existing.Comment = comment;
            existing.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.Feedbacks.Add(new Feedback
            {
                UserId = userId,
                Type = "AnswerRating",
                Rating = rating,
                Comment = comment,
                ChatHistoryId = chatHistoryId,
                CreatedAt = DateTime.UtcNow
            });
        }

        _context.SaveChanges();
        return Json(new { success = true });
    }

    // Form đánh giá chung website/model
    [HttpGet]
    public IActionResult Submit() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Submit(int rating, string? comment)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "guest";

        _context.Feedbacks.Add(new Feedback
        {
            UserId = userId,
            Type = "General",
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        });
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá!";
        return RedirectToAction("Submit");
    }

    // Trang Admin xem toàn bộ đánh giá
    [Authorize(Roles = "Admin")]
    public IActionResult Index(string? type)
    {
        var query = _context.Feedbacks.AsQueryable();
        if (!string.IsNullOrEmpty(type))
            query = query.Where(f => f.Type == type);

        var list = query.OrderByDescending(f => f.CreatedAt).ToList();

        ViewBag.AverageGeneralRating = _context.Feedbacks
            .Where(f => f.Type == "General")
            .Select(f => (double?)f.Rating)
            .Average() ?? 0;

        ViewBag.AnswerLikeCount = _context.Feedbacks.Count(f => f.Type == "AnswerRating" && f.Rating > 0);
        ViewBag.AnswerDislikeCount = _context.Feedbacks.Count(f => f.Type == "AnswerRating" && f.Rating < 0);

        return View(list);
    }
}