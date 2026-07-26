namespace GYM_MANAGEMENT_SYSTEM.AI.Models;

public class FaqEmbeddingEntry
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public float[] Vector { get; set; } = Array.Empty<float>();
}