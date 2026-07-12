using System.Text.Json.Serialization;

namespace AppraisalSystem.Application.Features.ChatUI;

public class CrawlDirectResult : CrawlResult
{
    [JsonPropertyName("crawl_result")]
    public CrawlResult? CrawlResult { get; set; }
}