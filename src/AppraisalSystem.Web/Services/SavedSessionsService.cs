using System.Text.Json;
using AppraisalSystem.Application.Features.ChatUI;
using Microsoft.JSInterop;

namespace AppraisalSystem.Web.Services;

public class SavedSessionsService(IJSRuntime js) : IAsyncDisposable
{
    private const string IndexKey = "rds-session-index";
    private const string SessionPrefix = "rds-session-";
    private const string ApprovalMapKey = "rds-approval-map";

    private Task<IJSObjectReference>? _module;
    private List<SavedSession> _sessions = new();
    private HashSet<string> _knownIds = new();
    private bool _skipStorageLoad;

    public event Action? OnChange;

    public IReadOnlyList<SavedSession> Sessions => _sessions.AsReadOnly();
    public int TotalListings => _sessions.Sum(s => s.Listings.Count);

    // Last OCR result — disimpan langsung agar tidak bergantung alur session
    public AddressInfo? LastOcrAddress { get; set; }
    public CertificatePayload? LastOcrCertificate { get; set; }

    public async Task InitializeAsync()
    {
        if (_skipStorageLoad)
        {
            _skipStorageLoad = false;
        }
        else
        {
            await LoadFromStorageAsync();
        }

        _module = js.InvokeAsync<IJSObjectReference>("import", "./js/saved-sessions.js").AsTask();
        var module = await _module;
        var dotnetRef = DotNetObjectReference.Create(this);
        await module.InvokeVoidAsync("listenStorage", IndexKey, dotnetRef);
        Notify();
    }

    public void LoadSessionsFromJson(string json)
    {
        _skipStorageLoad = true;
        LastOcrAddress = null;
        LastOcrCertificate = null;

        try
        {
            var sessions = JsonSerializer.Deserialize<List<SavedSession>>(json, JsonOptions);
            if (sessions is { Count: > 0 })
            {
                _sessions = sessions;
                _knownIds = sessions.Select(s => s.Id).ToHashSet();
                return;
            }
        }
        catch { /* fallback ke loadFromRepository */ }

        _sessions = new();
        _knownIds = new();
        Notify();
    }

    public void LoadFromRepository(List<PropertyListing> listings)
    {
        _skipStorageLoad = true;
        LastOcrAddress = null;
        LastOcrCertificate = null;

        if (listings.Count == 0)
        {
            _sessions = new();
            _knownIds = new();
            Notify();
            return;
        }

        var sessionId = GenerateSessionId();
        _sessions = new List<SavedSession>
        {
            new()
            {
                Id = sessionId,
                Label = LabelFromAddress(null),
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Listings = listings
            }
        };
        _knownIds = new HashSet<string> { sessionId };
        Notify();
    }

    [JSInvokable]
    public async Task OnStorageChanged()
    {
        await LoadFromStorageAsync();
        Notify();
    }

    private async Task LoadFromStorageAsync()
    {
        var indexJson = await js.InvokeAsync<string?>("localStorage.getItem", IndexKey);
        List<string> ids;
        try { ids = !string.IsNullOrEmpty(indexJson)
                ? JsonSerializer.Deserialize<List<string>>(indexJson, JsonOptions) ?? new()
                : new(); }
        catch { ids = new(); }

        if (ids.Count == 0) { _sessions = new(); _knownIds = new(); return; }

        var loaded = new List<SavedSession>();
        foreach (var id in ids)
        {
            var raw = await js.InvokeAsync<string?>("localStorage.getItem", SessionKey(id));
            if (string.IsNullOrEmpty(raw)) continue;
            try
            {
                var session = JsonSerializer.Deserialize<SavedSession>(raw, JsonOptions);
                if (session is not null) loaded.Add(session);
            }
            catch { /* skip corrupt entry */ }
        }

        _sessions = loaded;
        _knownIds = ids.ToHashSet();
    }

    private async Task PersistAsync()
    {
        var currentIds = _sessions.Select(s => s.Id).ToHashSet();

        // Remove stale keys from localStorage
        foreach (var stale in _knownIds.Except(currentIds))
            await js.InvokeVoidAsync("localStorage.removeItem", SessionKey(stale));

        // Save each session under its own key
        foreach (var session in _sessions)
        {
            var raw = JsonSerializer.Serialize(session, JsonOptions);
            await js.InvokeVoidAsync("localStorage.setItem", SessionKey(session.Id), raw);
        }

        // Update index with current session IDs
        var indexJson = JsonSerializer.Serialize(_sessions.Select(s => s.Id).ToList(), JsonOptions);
        await js.InvokeVoidAsync("localStorage.setItem", IndexKey, indexJson);

        _knownIds = currentIds;
        Notify();
    }

    private static string SessionKey(string sessionId) => $"{SessionPrefix}{sessionId}";

    public async Task AddToSessionAsync(string sessionId, PropertyListing listing, AddressInfo? address = null, CertificatePayload? certificate = null)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);

        if (session is null)
        {
            // Jika sudah ada session (misal dari DB), pakai session pertama
            if (_sessions.Count > 0)
            {
                session = _sessions[0];
                // Update address & certificate dengan data terbaru dari OCR
                if (address is not null) session.Address = address;
                if (certificate is not null) session.Certificate = certificate;
            }
            else
            {
                session = new SavedSession
                {
                    Id = sessionId,
                    Label = LabelFromAddress(address),
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    Address = address,
                    Certificate = certificate,
                };
                _sessions.Insert(0, session);
            }
        }
        else
        {
            // Update address & certificate if available
            if (address is not null) session.Address = address;
            if (certificate is not null) session.Certificate = certificate;
        }

        if (session.Listings.All(l => l.Url != listing.Url))
            session.Listings.Add(listing);
        await PersistAsync();
    }

    public async Task RemoveFromSessionAsync(string sessionId, string url)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session is null) return;
        session.Listings.RemoveAll(l => l.Url == url);
        // Jangan hapus session — biarkan tetap ada meskipun kosong
        await PersistAsync();
    }

    public async Task RemoveEntireSessionAsync(string sessionId)
    {
        _sessions.RemoveAll(s => s.Id == sessionId);
        await PersistAsync();
    }

    public async Task SaveApprovalMapAsync(Dictionary<string, int> map)
    {
        var json = JsonSerializer.Serialize(map, JsonOptions);
        await js.InvokeVoidAsync("localStorage.setItem", ApprovalMapKey, json);
    }

    public async Task<Dictionary<string, int>> LoadApprovalMapAsync()
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", ApprovalMapKey);
        if (string.IsNullOrEmpty(json) || json == "{}")
            return new();
        try { return JsonSerializer.Deserialize<Dictionary<string, int>>(json, JsonOptions) ?? new(); }
        catch { return new(); }
    }

    public async Task PersistFromMemoryAsync()
    {
        // Simpan state saat ini (misal approval status) ke localStorage tanpa hapus data
        var currentIds = _sessions.Select(s => s.Id).ToHashSet();
        foreach (var stale in _knownIds.Except(currentIds))
            await js.InvokeVoidAsync("localStorage.removeItem", SessionKey(stale));
        foreach (var session in _sessions)
        {
            var raw = JsonSerializer.Serialize(session, JsonOptions);
            await js.InvokeVoidAsync("localStorage.setItem", SessionKey(session.Id), raw);
        }
        var indexJson = JsonSerializer.Serialize(_sessions.Select(s => s.Id).ToList(), JsonOptions);
        await js.InvokeVoidAsync("localStorage.setItem", IndexKey, indexJson);
        _knownIds = currentIds;
        Notify();
    }

    public async Task ClearAllAsync()
    {
        // Remove all session keys
        foreach (var id in _knownIds)
            await js.InvokeVoidAsync("localStorage.removeItem", SessionKey(id));
        // Remove index
        await js.InvokeVoidAsync("localStorage.removeItem", IndexKey);
        _sessions.Clear();
        _knownIds.Clear();
        LastOcrAddress = null;
        LastOcrCertificate = null;
        Notify();
    }

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            var module = await _module;
            await module.DisposeAsync();
        }
    }

    public static string GenerateSessionId()
    {
        var ts = Convert.ToBase64String(BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var rand = Guid.NewGuid().ToString("N")[..5];
        return $"{ts}{rand}";
    }

    private static string LabelFromAddress(AddressInfo? addr)
    {
        if (addr is null) return "Sertifikat";
        var parts = new[] { addr.SubDistrict, addr.District, addr.City }
            .Where(s => !string.IsNullOrEmpty(s))
            .Take(2)
            .ToArray();
        return parts.Length > 0 ? string.Join(", ", parts) : "Sertifikat";
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };
}