using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class CreateSessionResult
{
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; } = "";
}