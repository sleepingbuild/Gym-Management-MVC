using GYM_MANAGEMENT_SYSTEM.AI.Models;
using GYM_MANAGEMENT_SYSTEM.Data;
using System.Text.Json;

namespace GYM_MANAGEMENT_SYSTEM.AI.Services;

public class KnowledgeBaseService
{
    private readonly ApplicationDbContext _context;
    private readonly IGymAiClient _aiClient;
    private static List<FaqEmbeddingEntry>? _cachedEmbeddings;

    private const double HIGH_SIMILARITY_THRESHOLD = 0.85;
    private const double LOW_SIMILARITY_THRESHOLD = 0.55;

    public KnowledgeBaseService(ApplicationDbContext context, IGymAiClient aiClient)
    {
        _context = context;
        _aiClient = aiClient;
        _cachedEmbeddings ??= LoadEmbeddings();
    }

    private static List<FaqEmbeddingEntry> LoadEmbeddings()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "faq_embeddings.json");
        if (!File.Exists(path)) return new List<FaqEmbeddingEntry>();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<FaqEmbeddingEntry>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        double dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB) + 1e-9);
    }

    public string? SearchAnswerExact(string question)
    {
        var faq = _context.FAQs.FirstOrDefault(x =>
            x.Question.Contains(question) || question.Contains(x.Question));
        return faq?.Answer;
    }

    public async Task<(string? Answer, string Source)> SearchAnswerAsync(string question)
    {
        if (_cachedEmbeddings == null || _cachedEmbeddings.Count == 0)
            return (null, "NoEmbeddings");

        var queryVector = await _aiClient.EmbedAsync(question);

        var best = _cachedEmbeddings
            .Select(e => new { Entry = e, Score = CosineSimilarity(queryVector, e.Vector) })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (best == null) return (null, "NoMatch");

        // TẠM THỜI: log ra để xem điểm số thật
        Console.WriteLine($"[RAG DEBUG] Q='{question}' | BestMatch='{best.Entry.Question}' | Score={best.Score:F4}");

        if (best.Score >= HIGH_SIMILARITY_THRESHOLD)
            return (best.Entry.Answer, $"KB_Direct (score={best.Score:F2})");

        if (best.Score >= LOW_SIMILARITY_THRESHOLD)
            return (best.Entry.Answer, $"KB_Context (score={best.Score:F2})");

        return (null, $"NoMatch (best_score={best.Score:F2})");
    }
}