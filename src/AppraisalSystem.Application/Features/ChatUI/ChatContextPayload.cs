using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class ChatContextPayload
{
    [JsonPropertyName("address")]
    public AddressInfo? Address { get; set; }
    [JsonPropertyName("certificate")]
    public CertificatePayload? Certificate { get; set; }
    [JsonPropertyName("params")]
    public Dictionary<string, string>? Params { get; set; }
}