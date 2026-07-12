using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class SavedSession
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    [JsonPropertyName("address")]
    public AddressInfo? Address { get; set; }

    [JsonPropertyName("listings")]
    public List<PropertyListing> Listings { get; set; } = new();

    [JsonPropertyName("certificate")]
    public CertificatePayload? Certificate { get; set; }
}