using System.ComponentModel.DataAnnotations;
using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Dtos;

public sealed class AppraisalUpsertDto
{
    public int? Id { get; set; }

    [MaxLength(40)]
    public string ApplicationNumber { get; set; } = string.Empty;

    public CustomerSegment Segment { get; set; } = CustomerSegment.Unknown;

    [MaxLength(50)]
    public string MakerId { get; set; } = string.Empty;

    [MaxLength(20)]
    public string BranchCode { get; set; } = string.Empty;

    public DateTime? ApplicationDateUtc { get; set; }

    public ApplicantType ApplicantType { get; set; } = ApplicantType.Unknown;

    [MaxLength(60)]
    public string DeedNumber { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public DateTime? BusinessEstablishedDate { get; set; }

    [MaxLength(80)]
    public string LegalEntity { get; set; } = string.Empty;

    [MaxLength(80)]
    public string BirthPlace { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(30)]
    public string MaritalStatus { get; set; } = string.Empty;

    [MaxLength(250)]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(250)]
    public string AddressLine2 { get; set; } = string.Empty;

    [MaxLength(250)]
    public string AddressLine3 { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Rt { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Rw { get; set; } = string.Empty;

    [MaxLength(10)]
    public string PostalCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Subdistrict { get; set; } = string.Empty;

    [MaxLength(100)]
    public string District { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(30)]
    public string MobileNumber1 { get; set; } = string.Empty;

    [MaxLength(30)]
    public string MobileNumber2 { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string DebtorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string IdentityNumber { get; set; } = string.Empty;

    [Required]
    public CollateralType CollateralType { get; set; } = CollateralType.Property;

    [MaxLength(100)]
    public string CollateralSubtype { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999999", ErrorMessage = "Nilai pasar harus antara 0 dan 999999999999.")]
    public decimal MarketValue { get; set; }

    [Range(typeof(decimal), "0", "999999999999", ErrorMessage = "Nilai likuidasi harus antara 0 dan 999999999999.")]
    public decimal LiquidationValue { get; set; }

    [MaxLength(4000)]
    public string Notes { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string InternalMemo { get; set; } = string.Empty;

    [MaxLength(16000)]
    public string WorkflowHistoryJson { get; set; } = string.Empty;
}