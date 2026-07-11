using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Dtos;

public sealed class AppraisalQueryDto
{
    public string? Search { get; init; }
    public AppraisalStatus? Status { get; init; }
    public ApplicantType? ApplicantType { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}