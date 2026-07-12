using AppraisalSystem.Application.Features.ChatUI;

namespace AppraisalSystem.Application.Features.ChatUI;

public class Message
{
    public string Id { get; set; } = "";
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
    public string? ImageUrl { get; set; }
    public AddressInfo? AddressInfo { get; set; }
    public List<PropertyListing>? Listings { get; set; }
    public string? CrawlUrl { get; set; }
    public int? CrawlTotal { get; set; }
    public string? CrawlError { get; set; }
    public MarketEstimateResult? MarketEstimate { get; set; }
    public CertificatePayload? CertificateInfo { get; set; }
}