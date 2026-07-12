using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class CreateChatSessionResult
{
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}