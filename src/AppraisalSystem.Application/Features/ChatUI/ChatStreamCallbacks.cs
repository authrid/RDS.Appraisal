namespace AppraisalSystem.Application.Features.ChatUI;

public class ChatStreamCallbacks
{
    public Action<string>? OnDelta { get; set; }
    public Action<string>? OnDone { get; set; }
    public Action<string>? OnError { get; set; }
    public Action<string, string>? OnStage { get; set; }
    public Action<List<PropertyListing>, int?, string?>? OnListings { get; set; }
    public Action<AddressInfo>? OnAddress { get; set; }
    public Action<MarketEstimateResult>? OnMarketEstimate { get; set; }
}