using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class ChatMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public ChatRole Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("listings")]
    public List<PropertyListing>? Listings { get; set; }

    [JsonPropertyName("addressInfo")]
    public AddressInfo? AddressInfo { get; set; }

    [JsonPropertyName("crawlUrl")]
    public string? CrawlUrl { get; set; }

    [JsonPropertyName("crawlTotal")]
    public int? CrawlTotal { get; set; }

    [JsonPropertyName("crawlError")]
    public string? CrawlError { get; set; }

    [JsonPropertyName("marketEstimate")]
    public MarketEstimateResult? MarketEstimate { get; set; }

    [JsonPropertyName("certificateInfo")]
    public CertificatePayload? CertificateInfo { get; set; }
}