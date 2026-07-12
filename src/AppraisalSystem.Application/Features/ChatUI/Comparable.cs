using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class Comparable
{
    [JsonPropertyName("source_url")]
    public string SourceUrl { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("price")]
    public double Price { get; set; }

    [JsonPropertyName("land_area_m2")]
    public double LandAreaM2 { get; set; }
}