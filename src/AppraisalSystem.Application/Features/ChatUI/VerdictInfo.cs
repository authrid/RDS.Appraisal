using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class VerdictInfo
{
    [JsonPropertyName("status")]
    public VerdictStatus Status { get; set; }

    [JsonPropertyName("deviation_percent")]
    public double DeviationPercent { get; set; }
}