namespace AppraisalSystem.Application.Dtos;

public sealed class DashboardSummaryDto
{
    public int Total { get; init; }
    public int Pending { get; init; }
    public int Approved { get; init; }
    public int Rejected { get; init; }
}