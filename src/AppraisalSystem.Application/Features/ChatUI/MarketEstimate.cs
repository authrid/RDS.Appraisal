using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class MarketEstimate
{
    [JsonPropertyName("estimated_value_min")]
    public double EstimatedValueMin { get; set; }

    [JsonPropertyName("estimated_value_median")]
    public double EstimatedValueMedian { get; set; }

    [JsonPropertyName("estimated_value_max")]
    public double EstimatedValueMax { get; set; }
}