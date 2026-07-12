using AppraisalSystem.Domain.Enums;

namespace AppraisalSystem.Application.Common;

public static class AppraisalStageReference
{
    public const string Stage1Code = "1.0";
    public const string Stage2Code = "2.0";
    public const string Stage3Code = "3.0";

    public const string Stage1Label = "Pencarian Data Maker";
    public const string Stage2Label = "Pencarian Data Checker";
    public const string Stage3Label = "Done";

    public static string GetCode(AppraisalStatus status) => status switch
    {
        AppraisalStatus.Draft => Stage1Code,
        AppraisalStatus.Submitted => Stage2Code,
        AppraisalStatus.Approved or AppraisalStatus.Rejected => Stage3Code,
        _ => Stage1Code
    };

    public static string GetLabel(AppraisalStatus status) => status switch
    {
        AppraisalStatus.Draft => Stage1Label,
        AppraisalStatus.Submitted => Stage2Label,
        AppraisalStatus.Approved or AppraisalStatus.Rejected => Stage3Label,
        _ => Stage1Label
    };

    public static string GetDisplay(AppraisalStatus status)
        => $"{GetCode(status)} - {GetLabel(status)}";
}