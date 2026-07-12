using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class CrawlResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("listings")]
    public List<PropertyListing>? Listings { get; set; }

    [JsonPropertyName("total_listings")]
    public int? TotalListings { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}