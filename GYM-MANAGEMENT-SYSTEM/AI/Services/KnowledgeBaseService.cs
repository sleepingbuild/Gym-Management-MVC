using GYM_MANAGEMENT_SYSTEM.AI.Models;
using GYM_MANAGEMENT_SYSTEM.Data;
using System.Text.Json;

namespace GYM_MANAGEMENT_SYSTEM.AI.Services;

public class KnowledgeBaseService
{
    private readonly ApplicationDbContext _context;

    public KnowledgeBaseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public string SearchAnswer(string question)
    {
        var faq = _context.FAQs
            .FirstOrDefault(x =>
                x.Question.Contains(question)
                ||
                question.Contains(x.Question));

        return faq?.Answer
            ?? "Xin lỗi, tôi chưa có dữ liệu cho câu hỏi này.";
    }
}