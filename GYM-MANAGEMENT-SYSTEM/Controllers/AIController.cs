using GYM_MANAGEMENT_SYSTEM.AI.Services;
using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Controllers;

public class AIController : Controller
{
    private readonly KnowledgeBaseService _kb;
    private readonly ApplicationDbContext _context;
    public AIController(KnowledgeBaseService kb, ApplicationDbContext context)
    {
        _kb = kb;
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
    public IActionResult Chat(ChatViewModel model)
    {
        model.Answer = _kb.SearchAnswer(model.Question);

        var history = new ChatHistory
        {
            UserId = "guest",
            Question = model.Question,
            Answer = model.Answer,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatHistories.Add(history);

        _context.SaveChanges();

        model.Histories = _context.ChatHistories
       .OrderByDescending(x => x.CreatedAt)
       .Take(20)
       .ToList();

        return View(model);
    }

}