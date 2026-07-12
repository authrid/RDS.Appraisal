using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class Verdict
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("deviation_percent")]
    public double DeviationPercent { get; set; }
}