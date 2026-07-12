using System.Net.Http.Headers;
using System.Text.Json;
using AppraisalSystem.Application.Features.ChatUI;

namespace AppraisalSystem.Web.Services;

public class OrchestratorClientService(HttpClient http)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<ProcessCertificateResult> ProcessCertificateAsync(
        Stream imageStream, string fileName, string certificateType, string? keyword, ProcessCertificateRefineParams? refineParams)
    {
        using var form = new MultipartFormDataContent();
        form.Add(CreateFileContent(imageStream, fileName), "image", fileName);
        form.Add(new StringContent(certificateType), "certificate_type");
        if (!string.IsNullOrEmpty(keyword))
            form.Add(new StringContent(keyword), "keyword");
        AppendRefineParams(form, refineParams);

        var response = await http.PostAsync("api/process-certificate", form);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProcessCertificateResult>(JsonOpts))!;
    }

    public async Task<CrawlDirectResult> CrawlDirectAsync(CrawlDirectParams? crawlParams)
    {
        using var form = new MultipartFormDataContent();
        AppendCrawlParams(form, crawlParams);
        var response = await http.PostAsync("api/crawl-direct", form);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CrawlDirectResult>(JsonOpts))!;
    }

    private static StreamContent CreateFileContent(Stream stream, string fileName)
    {
        var content = new StreamContent(stream);
        var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
        content.Headers.ContentType = new MediaTypeHeaderValue(ext switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg",
        });
        return content;
    }

    private static void AppendRefineParams(MultipartFormDataContent form, ProcessCertificateRefineParams? p)
    {
        if (p is null) return;
        if (!string.IsNullOrEmpty(p.Kt)) form.Add(new StringContent(p.Kt), "kt");
        if (!string.IsNullOrEmpty(p.Km)) form.Add(new StringContent(p.Km), "km");
        if (!string.IsNullOrEmpty(p.LtMin)) form.Add(new StringContent(p.LtMin), "lt_min");
        if (!string.IsNullOrEmpty(p.LtMax)) form.Add(new StringContent(p.LtMax), "lt_max");
        if (!string.IsNullOrEmpty(p.LbMin)) form.Add(new StringContent(p.LbMin), "lb_min");
        if (!string.IsNullOrEmpty(p.LbMax)) form.Add(new StringContent(p.LbMax), "lb_max");
        if (!string.IsNullOrEmpty(p.Hadap)) form.Add(new StringContent(p.Hadap), "hadap");
        if (!string.IsNullOrEmpty(p.City)) form.Add(new StringContent(p.City), "city");
        if (!string.IsNullOrEmpty(p.District)) form.Add(new StringContent(p.District), "district");
        if (!string.IsNullOrEmpty(p.TransactionType)) form.Add(new StringContent(p.TransactionType), "transaction_type");
    }

    private static void AppendCrawlParams(MultipartFormDataContent form, CrawlDirectParams? p)
    {
        if (p is null) return;
        if (!string.IsNullOrEmpty(p.Address)) form.Add(new StringContent(p.Address), "address");
        if (!string.IsNullOrEmpty(p.CertificateType)) form.Add(new StringContent(p.CertificateType), "certificate_type");
        if (!string.IsNullOrEmpty(p.Keyword)) form.Add(new StringContent(p.Keyword), "keyword");
        if (!string.IsNullOrEmpty(p.Kt)) form.Add(new StringContent(p.Kt), "kt");
        if (!string.IsNullOrEmpty(p.Km)) form.Add(new StringContent(p.Km), "km");
        if (!string.IsNullOrEmpty(p.LtMin)) form.Add(new StringContent(p.LtMin), "lt_min");
        if (!string.IsNullOrEmpty(p.LtMax)) form.Add(new StringContent(p.LtMax), "lt_max");
        if (!string.IsNullOrEmpty(p.LbMin)) form.Add(new StringContent(p.LbMin), "lb_min");
        if (!string.IsNullOrEmpty(p.LbMax)) form.Add(new StringContent(p.LbMax), "lb_max");
        if (!string.IsNullOrEmpty(p.Hadap)) form.Add(new StringContent(p.Hadap), "hadap");
        if (!string.IsNullOrEmpty(p.City)) form.Add(new StringContent(p.City), "city");
        if (!string.IsNullOrEmpty(p.District)) form.Add(new StringContent(p.District), "district");
        if (!string.IsNullOrEmpty(p.TransactionType)) form.Add(new StringContent(p.TransactionType), "transaction_type");
    }

    public async Task<CreateSessionResult> CreateChatSessionAsync(string? title)
    {
        var response = await http.PostAsJsonAsync("api/chat/session", new { title = title ?? "Chat" }, JsonOpts);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateSessionResult>(body, JsonOpts);
        if (result is null || string.IsNullOrEmpty(result.SessionId))
        {
            var snippet = (body?.Length > 200 ? body[..200] : body) ?? "(null)";
            throw new InvalidOperationException($"Server mengembalikan sessionId kosong. Response: {snippet}");
        }
        return result;
    }

    public async Task UpdateSessionContextAsync(string sessionId, ChatContextPayload payload)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            Console.Error.WriteLine("[OrchestratorClient] UpdateSessionContextAsync skipped: sessionId kosong");
            return;
        }
        var body = new Dictionary<string, object?>
        {
            ["session_id"] = sessionId,
        };
        if (payload.Address is not null) body["address"] = payload.Address;
        if (payload.Certificate is not null) body["certificate"] = payload.Certificate;
        if (payload.Params is not null) body["params"] = payload.Params;

        var request = new HttpRequestMessage(HttpMethod.Patch, $"api/chat/session/{sessionId}/context")
        {
            Content = JsonContent.Create(body, options: JsonOpts),
        };
        var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<(bool Success, List<object> Messages)> GetChatMessagesAsync(string sessionId)
    {
        var response = await http.GetAsync($"api/chat/{sessionId}/messages");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var success = result.TryGetProperty("success", out var s) && s.GetBoolean();
        var messages = result.TryGetProperty("messages", out var m)
            ? JsonSerializer.Deserialize<List<object>>(m.GetRawText(), JsonOpts) ?? new()
            : new List<object>();
        return (success, messages);
    }

    public async Task SendChatMessageAsync(
        string sessionId,
        string message,
        Stream? imageStream,
        string? imageFileName,
        Action<string>? onDelta,
        Action<string, string>? onStage,
        Action<string>? onDone,
        Action<string>? onError,
        Action<List<PropertyListing>, int?, string?>? onListings,
        Action<AddressInfo>? onAddress,
        Action<JsonElement>? onMarketEstimate,
        CancellationToken ct = default)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(sessionId), "sessionId");
            form.Add(new StringContent(message), "message");
            if (imageStream is not null && imageFileName is not null)
                form.Add(CreateFileContent(imageStream, imageFileName), "image", imageFileName);

            using var request = new HttpRequestMessage(HttpMethod.Post, "api/chat")
            {
                Content = form,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(ct);
                onError?.Invoke($"Service error ({(int)response.StatusCode}): {errorText[..Math.Min(errorText.Length, 200)]}");
                return;
            }

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream);

            var currentEvent = "";
            var currentData = "";

            while (!ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(ct);
                if (line is null) break;

                if (line.StartsWith("event: "))
                {
                    currentEvent = line[7..].Trim();
                }
                else if (line.StartsWith("data: "))
                {
                    currentData = line[6..].Trim();
                }
                else if (line.Length == 0)
                {
                    if (!string.IsNullOrEmpty(currentEvent) && !string.IsNullOrEmpty(currentData))
                    {
                        try
                        {
                            using var doc = JsonDocument.Parse(currentData);
                            var root = doc.RootElement;

                            switch (currentEvent)
                            {
                                case "delta" when onDelta is not null:
                                    if (root.TryGetProperty("text", out var t))
                                        onDelta(t.GetString() ?? "");
                                    break;
                                case "stage" when onStage is not null:
                                    onStage(
                                        root.TryGetProperty("stage", out var s) ? s.GetString() ?? "" : "",
                                        root.TryGetProperty("label", out var l) ? l.GetString() ?? "" : "");
                                    break;
                                case "done":
                                    onDone?.Invoke(root.TryGetProperty("sessionId", out var sid) ? sid.GetString() ?? sessionId : sessionId);
                                    break;
                                case "error" when onError is not null:
                                    onError(root.TryGetProperty("error", out var e) ? e.GetString() ?? "Unknown error" : "Unknown error");
                                    break;
                                case "listings" when onListings is not null:
                                    {
                                        var listings = root.TryGetProperty("listings", out var ls)
                                            ? JsonSerializer.Deserialize<List<PropertyListing>>(ls.GetRawText(), JsonOpts) ?? new()
                                            : new List<PropertyListing>();
                                        var total = root.TryGetProperty("total", out var tot) ? tot.GetInt32() : (int?)null;
                                        var url = root.TryGetProperty("url", out var u) ? u.GetString() : null;
                                        onListings(listings, total, url);
                                        break;
                                    }
                                case "address" when onAddress is not null:
                                    {
                                        var addr = JsonSerializer.Deserialize<AddressInfo>(root.GetRawText(), JsonOpts);
                                        if (addr is not null) onAddress(addr);
                                        break;
                                    }
                                case "market_estimate" when onMarketEstimate is not null:
                                    onMarketEstimate(root);
                                    break;
                            }
                        }
                        catch { /* skip malformed SSE */ }
                    }
                    currentEvent = "";
                    currentData = "";
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message);
        }
    }

    public static string GenerateId()
    {
        return $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Guid.NewGuid().ToString("N")[..7]}";
    }

    public static ProcessCertificateRefineParams ParseRefineFromText(string text)
    {
        var result = new ProcessCertificateRefineParams();
        var lower = text.ToLowerInvariant();

        var ltRange = System.Text.RegularExpressions.Regex.Match(lower,
            @"(?:luas\s+tanah|lt)\s*(?::)?\s*(\d+)(?:\s*m[2²])?\s*(?:-|sampai\s+dengan|sampai|s\/d)\s*(\d+)");
        if (ltRange.Success)
        {
            result.LtMin = ltRange.Groups[1].Value;
            result.LtMax = ltRange.Groups[2].Value;
        }
        else
        {
            var ltSingle = System.Text.RegularExpressions.Regex.Match(lower, @"(?:luas\s+tanah|lt)\s*(?::)?\s*(\d+)");
            if (ltSingle.Success) { result.LtMin = ltSingle.Groups[1].Value; result.LtMax = ltSingle.Groups[1].Value; }
        }

        var lbRange = System.Text.RegularExpressions.Regex.Match(lower,
            @"(?:luas\s+bangunan|lb)\s*(?::)?\s*(\d+)(?:\s*m[2²])?\s*(?:-|sampai\s+dengan|sampai|s\/d)\s*(\d+)");
        if (lbRange.Success) { result.LbMin = lbRange.Groups[1].Value; result.LbMax = lbRange.Groups[2].Value; }

        var ktMatch = System.Text.RegularExpressions.Regex.Match(lower, @"(?:kamar\s+tidur|kt)\s*(?::)?\s*(\d+)");
        if (ktMatch.Success) result.Kt = ktMatch.Groups[1].Value;

        var kmMatch = System.Text.RegularExpressions.Regex.Match(lower, @"(?:kamar\s+mandi|km)\s*(?::)?\s*(\d+)");
        if (kmMatch.Success) result.Km = kmMatch.Groups[1].Value;

        result.TransactionType = System.Text.RegularExpressions.Regex.IsMatch(text, @"\b(disewa|rent|sewa)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            ? "disewa" : "dijual";

        return result;
    }
}