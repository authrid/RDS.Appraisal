namespace AppraisalSystem.Application.Dtos;

public sealed class AppraisalListItemDto
{
    public int Id { get; init; }
    public string ApplicationNumber { get; init; } = string.Empty;
    public string Segment { get; init; } = string.Empty;
    public string ApplicantType { get; init; } = string.Empty;
    public string IdentityNumber { get; init; } = string.Empty;
    public string DeedNumber { get; init; } = string.Empty;
    public DateTime? DateOfBirth { get; init; }
    public DateTime? BusinessEstablishedDate { get; init; }
    public string DebtorName { get; init; } = string.Empty;
    public string CollateralType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}