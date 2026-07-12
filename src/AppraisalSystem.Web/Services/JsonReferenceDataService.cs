using System.Text.Json;
using AppraisalSystem.Application.Dtos;
using AppraisalSystem.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace AppraisalSystem.Web.Services;

// JSON-backed implementation of IReferenceDataService. Membaca dari wwwroot/data/reference/*.json,
// dengan fallback ke legacy wwwroot/data/branch_data.json untuk cabang. In-memory cached 1 jam.
public sealed class JsonReferenceDataService(
    IWebHostEnvironment environment,
    IMemoryCache cache,
    ILogger<JsonReferenceDataService> logger) : IReferenceDataService
{
    private const string CachePrefix = "reference:";
    private const string ReferenceFolder = "data/reference";
    private const string LegacyBranchRelativePath = "data/branch_data.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly IReadOnlyList<ReferenceItemDto> Empty = Array.Empty<ReferenceItemDto>();

    public Task<IReadOnlyList<ReferenceItemDto>> GetCollateralSubtypesAsync(CancellationToken cancellationToken = default)
        => LoadAsync("collateral-subtype", cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetProvincesAsync(CancellationToken cancellationToken = default)
        => LoadAsync("province", cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetCountriesAsync(CancellationToken cancellationToken = default)
        => LoadAsync("country", cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetMaritalStatusesAsync(CancellationToken cancellationToken = default)
        => LoadAsync("marital-status", cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetGendersAsync(CancellationToken cancellationToken = default)
        => LoadAsync("gender", cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetLegalEntitiesAsync(CancellationToken cancellationToken = default)
        => LoadAsync("legal-entity", cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetCitiesAsync(string? parentProvinceCode = null, CancellationToken cancellationToken = default)
        => LoadFilteredAsync("city", parentProvinceCode, cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetDistrictsAsync(string? parentCityCode = null, CancellationToken cancellationToken = default)
        => LoadFilteredAsync("district", parentCityCode, cancellationToken);

    public Task<IReadOnlyList<ReferenceItemDto>> GetSubdistrictsAsync(string? parentDistrictCode = null, CancellationToken cancellationToken = default)
        => LoadFilteredAsync("subdistrict", parentDistrictCode, cancellationToken);

    public async Task<IReadOnlyList<ReferenceItemDto>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        var cached = await cache.GetOrCreateAsync<IReadOnlyList<ReferenceItemDto>>(
            $"{CachePrefix}branch",
            async entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);

                var primary = ResolvePath("branch");
                if (File.Exists(primary))
                {
                    return await ReadItemsAsync(primary, cancellationToken);
                }

                var root = environment.WebRootPath ?? string.Empty;
                var legacy = Path.Combine(root, LegacyBranchRelativePath);
                if (File.Exists(legacy))
                {
                    return await ReadBranchLegacyAsync(legacy, cancellationToken);
                }

                logger.LogWarning("Branch reference data not found at either {Primary} or {Legacy}.", primary, legacy);
                return Empty;
            });

        return cached ?? Empty;
    }

    private async Task<IReadOnlyList<ReferenceItemDto>> LoadFilteredAsync(
        string key,
        string? parentCode,
        CancellationToken cancellationToken)
    {
        var all = await LoadAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(parentCode))
        {
            return all;
        }

        var parent = parentCode.Trim();
        return all
            .Where(x => string.Equals(x.ParentCode, parent, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private async Task<IReadOnlyList<ReferenceItemDto>> LoadAsync(string key, CancellationToken cancellationToken)
    {
        var cached = await cache.GetOrCreateAsync<IReadOnlyList<ReferenceItemDto>>(
            $"{CachePrefix}{key}",
            async entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1);
                var path = ResolvePath(key);

                if (!File.Exists(path))
                {
                    logger.LogWarning("Reference data file not found: {Path}", path);
                    return Empty;
                }

                try
                {
                    return await ReadItemsAsync(path, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed reading reference data at {Path}", path);
                    return Empty;
                }
            });

        return cached ?? Empty;
    }

    private string ResolvePath(string key)
    {
        var root = environment.WebRootPath ?? string.Empty;
        return Path.Combine(root, ReferenceFolder, $"{key}.json");
    }

    private static async Task<IReadOnlyList<ReferenceItemDto>> ReadItemsAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var items = await JsonSerializer.DeserializeAsync<List<ReferenceItemDto>>(stream, JsonOptions, cancellationToken) ?? [];
        return Normalize(items);
    }

    private static async Task<IReadOnlyList<ReferenceItemDto>> ReadBranchLegacyAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var raw = await JsonSerializer.DeserializeAsync<List<LegacyBranchItem>>(stream, JsonOptions, cancellationToken) ?? [];

        var items = raw
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select((x, i) => new ReferenceItemDto
            {
                Code = (x.Code ?? string.Empty).Trim(),
                Label = (x.Name ?? string.Empty).Trim(),
                SortOrder = i,
                IsActive = true
            })
            .ToList();

        return Normalize(items);
    }

    private static IReadOnlyList<ReferenceItemDto> Normalize(IEnumerable<ReferenceItemDto> items)
    {
        return items
            .Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.Label))
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private sealed class LegacyBranchItem
    {
        public string? Code { get; init; }
        public string? Name { get; init; }
    }
}
