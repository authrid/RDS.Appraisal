using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

/// <summary>Address info extracted from certificate OCR — only administrative levels.</summary>
public class AddressInfo
{
    [JsonPropertyName("province")]
    public string Province { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("district")]
    public string District { get; set; } = string.Empty;

    [JsonPropertyName("sub_district")]
    public string SubDistrict { get; set; } = string.Empty;
}