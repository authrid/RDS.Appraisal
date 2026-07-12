using AppraisalSystem.Application.Common;
using AppraisalSystem.Application.Dtos;
using AppraisalSystem.Domain.Entities;
using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Interfaces;

public interface IAppraisalRepository
{
    Task<PagedResult<Appraisal>> GetPagedAsync(AppraisalQueryDto query, CancellationToken cancellationToken = default);
    Task<Appraisal?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Appraisal?> GetByPublicIdAsync(string publicId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Appraisal appraisal, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appraisal appraisal, CancellationToken cancellationToken = default);
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task SetStatusAsync(int id, AppraisalStatus status, string? supervisorNote, CancellationToken cancellationToken = default);

    Task ReplaceListingsAsync(int appraisalId, List<SavedPropertyListing> listings, CancellationToken cancellationToken = default);
    Task<List<SavedPropertyListing>> GetListingsAsync(int appraisalId, CancellationToken cancellationToken = default);
    Task UpdateListingApprovalAsync(int listingId, ListingApprovalStatus status, CancellationToken cancellationToken = default);
    Task SaveOcrResultAsync(int appraisalId, OcrResult ocr, CancellationToken cancellationToken = default);
    Task<OcrResult?> GetOcrResultAsync(int appraisalId, CancellationToken cancellationToken = default);
}