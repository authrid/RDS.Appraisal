using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Dtos;

public sealed class AppraisalQueryDto
{
    public string? Search { get; init; }
    public string? ApplicationNumber { get; init; }
    public string? DebtorName { get; init; }
    public string? CollateralSubtype { get; init; }
    public DateTime? ApplicationDateFromUtc { get; init; }
    public DateTime? ApplicationDateToUtc { get; init; }
    public AppraisalStatus? Status { get; init; }
    public ApplicantType? ApplicantType { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}