using System.Text.Json.Serialization;

namespace GYM_MANAGEMENT_SYSTEM.AI.Models;

public class GymFAQ
{
    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("answer")]
    public string Answer { get; set; } = string.Empty;
}