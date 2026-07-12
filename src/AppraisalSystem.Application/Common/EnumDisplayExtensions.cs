using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Common;

// Satu tempat untuk label enum. UI (Razor) memanggil enumValue.GetLabel(); jangan bikin switch label lokal.
public static class EnumDisplayExtensions
{
    public static string GetLabel(this ApplicantType value) => value switch
    {
        ApplicantType.Individual => "Perorangan",
        ApplicantType.Business => "Badan Usaha",
        _ => "-"
    };

    public static string GetLabel(this CustomerSegment value) => value switch
    {
        CustomerSegment.Consumer => "Consumer",
        CustomerSegment.Commercial => "Commercial",
        CustomerSegment.SME => "SME",
        CustomerSegment.Corporate => "Corporate",
        _ => "-"
    };

    public static string GetLabel(this CollateralType value) => value switch
    {
        CollateralType.Property => "Properti",
        CollateralType.Vehicle => "Kendaraan",
        CollateralType.Machine => "Mesin",
        CollateralType.Inventory => "Inventori",
        CollateralType.Other => "Lain-lain",
        _ => "-"
    };

    public static string GetLabel(this AppraisalStatus value) => value switch
    {
        AppraisalStatus.Draft => "Draft",
        AppraisalStatus.Submitted => "Submitted",
        AppraisalStatus.Approved => "Approved",
        AppraisalStatus.Rejected => "Rejected",
        _ => "-"
    };

    public static string GetLabel(this ListingApprovalStatus value) => value switch
    {
        ListingApprovalStatus.Pending => "Pending",
        ListingApprovalStatus.Approved => "Approved",
        ListingApprovalStatus.Rejected => "Rejected",
        _ => "-"
    };
}
