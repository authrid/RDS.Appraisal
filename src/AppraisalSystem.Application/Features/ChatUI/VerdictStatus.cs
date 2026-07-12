using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

[JsonConverter(typeof(JsonStringEnumConverter<VerdictStatus>))]
public enum VerdictStatus
{
    [JsonPropertyName("within_range")]
    WithinRange,
    [JsonPropertyName("above_range")]
    AboveRange,
    [JsonPropertyName("below_range")]
    BelowRange,
}