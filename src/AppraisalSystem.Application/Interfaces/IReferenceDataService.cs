using AppraisalSystem.Application.Dtos;

namespace AppraisalSystem.Application.Interfaces;

// Abstraksi lookup terpusat. Implementasi awal: JsonReferenceDataService (Web).
// Nanti bisa diganti ke DbReferenceDataService tanpa mengubah pemanggil.
public interface IReferenceDataService
{
    Task<IReadOnlyList<ReferenceItemDto>> GetCollateralSubtypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetProvincesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetCountriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetMaritalStatusesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetGendersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetLegalEntitiesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetBranchesAsync(CancellationToken cancellationToken = default);

    // Geo hierarchy (sample subset; ParentCode = code parent).
    // parentCode null/empty → semua item aktif.
    Task<IReadOnlyList<ReferenceItemDto>> GetCitiesAsync(string? parentProvinceCode = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetDistrictsAsync(string? parentCityCode = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReferenceItemDto>> GetSubdistrictsAsync(string? parentDistrictCode = null, CancellationToken cancellationToken = default);
}
