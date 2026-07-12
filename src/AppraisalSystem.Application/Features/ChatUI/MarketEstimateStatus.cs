using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

[JsonConverter(typeof(JsonStringEnumConverter<MarketEstimateStatus>))]
public enum MarketEstimateStatus
{
    [JsonPropertyName("ok")]
    Ok,
    [JsonPropertyName("insufficient_data")]
    InsufficientData,
    [JsonPropertyName("invalid_input")]
    InvalidInput,
}