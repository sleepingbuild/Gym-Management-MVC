using System.Text.Json;
using GYM_MANAGEMENT_SYSTEM.AI.Models;
using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.AI.Services;

public class DatasetImportService
{
    private readonly ApplicationDbContext _context;

    public DatasetImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public int ImportFAQs(string filePath)
    {
        string json = File.ReadAllText(filePath);

        var data = JsonSerializer.Deserialize<List<FAQImportModel>>(json);

        if (data == null)
            return 0;

        foreach (var item in data)
        {
            var faq = new FAQ
            {
                Question = item.Question,
                Answer = item.Answer,
                Category = item.Category
            };

            _context.FAQs.Add(faq);
        }

        _context.SaveChanges();

        return data.Count;
    }
}