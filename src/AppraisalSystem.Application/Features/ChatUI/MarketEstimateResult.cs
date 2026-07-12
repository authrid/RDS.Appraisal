using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class MarketEstimateResult
{
    [JsonPropertyName("status")]
    public MarketEstimateStatus Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("land_area_m2")]
    public double? LandAreaM2 { get; set; }

    [JsonPropertyName("location")]
    public LocationInfo? Location { get; set; }

    [JsonPropertyName("target_appraisal_value")]
    public double? TargetAppraisalValue { get; set; }

    [JsonPropertyName("market_estimate")]
    public MarketEstimateSummary? MarketEstimateSummary { get; set; }

    [JsonPropertyName("verdict")]
    public Verdict? Verdict { get; set; }

    [JsonPropertyName("comparables_used")]
    public List<MarketEstimateComparable>? ComparablesUsed { get; set; }

    [JsonPropertyName("disclaimer")]
    public string? Disclaimer { get; set; }
    
}