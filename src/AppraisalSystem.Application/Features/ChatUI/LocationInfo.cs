using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class LocationInfo
{
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("district")]
    public string? District { get; set; }
}