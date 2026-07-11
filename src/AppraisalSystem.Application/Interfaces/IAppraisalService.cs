using AppraisalSystem.Application.Common;
using AppraisalSystem.Application.Dtos;
using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Interfaces;

public interface IAppraisalService
{
    Task<PagedResult<AppraisalListItemDto>> GetPagedAsync(AppraisalQueryDto query, CancellationToken cancellationToken = default);
    Task<AppraisalDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(AppraisalUpsertDto request, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppraisalUpsertDto request, CancellationToken cancellationToken = default);
    Task SubmitAsync(int id, string? actor = null, CancellationToken cancellationToken = default);
    Task ApproveAsync(int id, string? supervisorNote, string? actor = null, CancellationToken cancellationToken = default);
    Task RejectAsync(int id, string? supervisorNote, string? actor = null, CancellationToken cancellationToken = default);
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}