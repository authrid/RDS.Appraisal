using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

/// <summary>Result from the backend /api/process-certificate endpoint.</summary>
public class ProcessCertificateResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("address")]
    public AddressInfo Address { get; set; } = new();

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("params_used")]
    public ProcessCertificateRefineParams? ParamsUsed { get; set; }

    [JsonPropertyName("crawl_result")]
    public CrawlResult? CrawlResult { get; set; }

    [JsonPropertyName("certificate")]
    public CertificatePayload? Certificate { get; set; }
}







