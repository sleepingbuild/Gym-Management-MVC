using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers;

public class AIController : Controller
{
    private readonly AIService _ai;
    private readonly ApplicationDbContext _context;

    public AIController(AIService ai, ApplicationDbContext context)
    {
        _ai = ai;
        _context = context;
    }

    [HttpGet]
    public IActionResult Chat()
    {
        var model = new ChatViewModel
        {
            Histories = _context.ChatHistories
                .OrderByDescending(x => x.CreatedAt)
                .Take(20)
                .ToList()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Chat(ChatViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Question))
            return RedirectToAction(nameof(Chat));

        // Lấy lịch sử gần nhất để truyền vào AI (context-aware)
        var recentHistory = _context.ChatHistories
            .OrderByDescending(x => x.CreatedAt)
            .Take(6)
            .ToList()
            .SelectMany(h => new[]
            {
                new ChatMessage { role = "user",      content = h.Question },
                new ChatMessage { role = "assistant", content = h.Answer   }
            })
            .ToList();

        // Gọi AI server
        model.Answer = await _ai.AskAsync(model.Question, recentHistory);

        // Lưu lịch sử
        _context.ChatHistories.Add(new ChatHistory
        {
            UserId = User.Identity?.Name ?? "guest",
            Question = model.Question,
            Answer = model.Answer,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        model.Histories = _context.ChatHistories
            .OrderByDescending(x => x.CreatedAt)
            .Take(20)
            .ToList();

        return View(model);
    }
}