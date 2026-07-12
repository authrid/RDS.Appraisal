using System.Text.Json;
using AppraisalSystem.Application.Features.ChatUI;
using Microsoft.JSInterop;

namespace AppraisalSystem.Web.Services;

public class SavedSessionsService(IJSRuntime js) : IAsyncDisposable
{
    private const string StorageKey = "rds-saved-sessions";

    private Task<IJSObjectReference>? _module;
    private DotNetObjectReference<SavedSessionsService>? _dotNetRef;
    private List<SavedSession> _sessions = new();
    private bool _disposed;

    public event Action? OnChange;

    public IReadOnlyList<SavedSession> Sessions => _sessions.AsReadOnly();
    public int TotalListings => _sessions.Sum(s => s.Listings.Count);

    public async Task InitializeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await LoadFromStorageAsync();
        try
        {
            _module = js.InvokeAsync<IJSObjectReference>("import", "./js/saved-sessions.js").AsTask();
            var module = await _module;
            _dotNetRef ??= DotNetObjectReference.Create(this);
            await module.InvokeVoidAsync("listenStorage", StorageKey, _dotNetRef);
        }
        catch (JSDisconnectedException)
        {
            // Circuit already disconnected; skip JS listener registration.
        }
        catch (ObjectDisposedException)
        {
            // Runtime was disposed during teardown.
        }

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
        string? json;

        try
        {
            json = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        }
        catch (JSDisconnectedException)
        {
            return;
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        if (!string.IsNullOrEmpty(json))
        {
            try { _sessions = JsonSerializer.Deserialize<List<SavedSession>>(json, JsonOptions) ?? new(); }
            catch { _sessions = new(); }
        }
        else { _sessions = new(); }
    }

    private async Task PersistAsync()
    {
        var json = JsonSerializer.Serialize(_sessions, JsonOptions);
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }
        catch (JSDisconnectedException)
        {
            return;
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        Notify();
    }

    public async Task AddToSessionAsync(string sessionId, PropertyListing listing, AddressInfo? address = null)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session is null)
        {
            session = new SavedSession
            {
                Id = sessionId,
                Label = LabelFromAddress(address),
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Address = address,
            };
            _sessions.Insert(0, session);
        }
        if (session.Listings.All(l => l.Url != listing.Url))
            session.Listings.Insert(0, listing);
        await PersistAsync();
    }

    public async Task RemoveFromSessionAsync(string sessionId, string url)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session is null) return;
        session.Listings.RemoveAll(l => l.Url == url);
        if (session.Listings.Count == 0)
            _sessions.Remove(session);
        await PersistAsync();
    }

    public async Task RemoveEntireSessionAsync(string sessionId)
    {
        _sessions.RemoveAll(s => s.Id == sessionId);
        await PersistAsync();
    }

    public async Task ClearAllAsync()
    {
        _sessions.Clear();
        await PersistAsync();
    }

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _dotNetRef?.Dispose();
        _dotNetRef = null;

        if (_module is not null)
        {
            try
            {
                var module = await _module;
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Expected when circuit disconnects before JS module disposal completes.
            }
            catch (ObjectDisposedException)
            {
                // Runtime/module already disposed.
            }
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