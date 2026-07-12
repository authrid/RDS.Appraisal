using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Domain.Entities;

public class Appraisal
{
    public int Id { get; set; }
    public string PublicId { get; set; } = Guid.NewGuid().ToString("D");
    public string ApplicationNumber { get; set; } = string.Empty;
    public CustomerSegment Segment { get; set; } = CustomerSegment.Unknown;
    public string MakerId { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public DateTime? ApplicationDateUtc { get; set; }

    public ApplicantType ApplicantType { get; set; } = ApplicantType.Unknown;
    public string DeedNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public DateTime? BusinessEstablishedDate { get; set; }
    public string LegalEntity { get; set; } = string.Empty;
    public string BirthPlace { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string MotherMaidenName { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string AddressLine3 { get; set; } = string.Empty;
    public string Rt { get; set; } = string.Empty;
    public string Rw { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Subdistrict { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public string MobileNumber1 { get; set; } = string.Empty;
    public string MobileNumber2 { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string DebtorName { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public CollateralType CollateralType { get; set; }
    public string CollateralSubtype { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Specification { get; set; } = string.Empty;
    public decimal MarketValue { get; set; }
    public decimal LiquidationValue { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string InternalMemo { get; set; } = string.Empty;
    public string WorkflowHistoryJson { get; set; } = string.Empty;
    public string SavedSessionJson { get; set; } = string.Empty;
    public AppraisalStatus Status { get; set; } = AppraisalStatus.Draft;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? SupervisorNote { get; set; }
}