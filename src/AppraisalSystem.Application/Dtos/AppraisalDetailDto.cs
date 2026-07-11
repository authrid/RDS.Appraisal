using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Dtos;

public sealed class AppraisalDetailDto
{
    public int Id { get; init; }
    public string ApplicationNumber { get; init; } = string.Empty;
    public CustomerSegment Segment { get; init; }
    public string MakerId { get; init; } = string.Empty;
    public string BranchCode { get; init; } = string.Empty;
    public DateTime? ApplicationDateUtc { get; init; }
    public ApplicantType ApplicantType { get; init; }
    public string DeedNumber { get; init; } = string.Empty;
    public DateTime? DateOfBirth { get; init; }
    public DateTime? BusinessEstablishedDate { get; init; }
    public string LegalEntity { get; init; } = string.Empty;
    public string BirthPlace { get; init; } = string.Empty;
    public string Gender { get; init; } = string.Empty;
    public string MaritalStatus { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = string.Empty;
    public string AddressLine2 { get; init; } = string.Empty;
    public string AddressLine3 { get; init; } = string.Empty;
    public string Rt { get; init; } = string.Empty;
    public string Rw { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Subdistrict { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string MobileNumber1 { get; init; } = string.Empty;
    public string MobileNumber2 { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string DebtorName { get; init; } = string.Empty;
    public string IdentityNumber { get; init; } = string.Empty;
    public CollateralType CollateralType { get; init; }
    public string CollateralSubtype { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Specification { get; init; } = string.Empty;
    public decimal MarketValue { get; init; }
    public decimal LiquidationValue { get; init; }
    public string Notes { get; init; } = string.Empty;
    public string InternalMemo { get; init; } = string.Empty;
    public string WorkflowHistoryJson { get; init; } = string.Empty;
    public AppraisalStatus Status { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public string? SupervisorNote { get; init; }
}