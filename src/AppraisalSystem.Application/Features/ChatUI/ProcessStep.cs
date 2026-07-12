using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

[JsonConverter(typeof(JsonStringEnumConverter<ProcessStep>))]
public enum ProcessStep
{
    [JsonPropertyName("idle")]
    Idle,
    [JsonPropertyName("uploading")]
    Uploading,
    [JsonPropertyName("ocr")]
    Ocr,
    [JsonPropertyName("crawling")]
    Crawling,
    [JsonPropertyName("done")]
    Done,
    [JsonPropertyName("error")]
    Error,
}