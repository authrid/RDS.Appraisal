using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class MarketEstimateSummary
{
    [JsonPropertyName("price_per_sqm_min")]
    public double PricePerSqmMin { get; set; }

    [JsonPropertyName("price_per_sqm_median")]
    public double PricePerSqmMedian { get; set; }

    [JsonPropertyName("price_per_sqm_max")]
    public double PricePerSqmMax { get; set; }

    [JsonPropertyName("estimated_value_min")]
    public double EstimatedValueMin { get; set; }

    [JsonPropertyName("estimated_value_median")]
    public double EstimatedValueMedian { get; set; }

    [JsonPropertyName("estimated_value_max")]
    public double EstimatedValueMax { get; set; }
}