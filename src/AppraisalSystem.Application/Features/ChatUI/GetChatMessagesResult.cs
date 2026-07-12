using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class GetChatMessagesResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("messages")]
    public List<object> Messages { get; set; } = new();
}