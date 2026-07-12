using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class MarketEstimateComparable
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public double Price { get; set; }

    [JsonPropertyName("land_area_m2")]
    public double LandAreaM2 { get; set; }

    [JsonPropertyName("building_area_m2")]
    public double? BuildingAreaM2 { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("source_url")]
    public string SourceUrl { get; set; } = string.Empty;

    [JsonPropertyName("price_per_sqm")]
    public double PricePerSqm { get; set; }
}