using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

/// <summary>Parameters used for the Brighton crawl.</summary>
public class CrawlParams
{
    [JsonPropertyName("certificate_type")]
    public string CertificateType { get; set; } = string.Empty;

    [JsonPropertyName("keyword")]
    public string Keyword { get; set; } = string.Empty;

    [JsonPropertyName("kt")]
    public string Kt { get; set; } = string.Empty;

    [JsonPropertyName("km")]
    public string Km { get; set; } = string.Empty;

    [JsonPropertyName("lt_min")]
    public string LtMin { get; set; } = string.Empty;

    [JsonPropertyName("lt_max")]
    public string LtMax { get; set; } = string.Empty;

    [JsonPropertyName("lb_min")]
    public string LbMin { get; set; } = string.Empty;

    [JsonPropertyName("lb_max")]
    public string LbMax { get; set; } = string.Empty;

    [JsonPropertyName("hadap")]
    public string Hadap { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("district")]
    public string District { get; set; } = string.Empty;

    [JsonPropertyName("transaction_type")]
    public string TransactionType { get; set; } = string.Empty;
}