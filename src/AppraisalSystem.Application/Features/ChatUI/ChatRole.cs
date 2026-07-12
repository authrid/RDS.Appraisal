using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

[JsonConverter(typeof(JsonStringEnumConverter<ChatRole>))]
public enum ChatRole
{
    [JsonPropertyName("user")]
    User,
    [JsonPropertyName("assistant")]
    Assistant,
}