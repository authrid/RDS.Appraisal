namespace AppraisalSystem.Application.Common;

// Single source of truth untuk role name yang dipakai di [Authorize], IsInRole, dan sidebar.
public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Appraiser = "Appraiser";
    public const string Supervisor = "Supervisor";
    public const string Checker = "Checker";

    public const string AppraiserOrAdmin = Appraiser + "," + Admin;
    public const string SupervisorOrChecker = Supervisor + "," + Checker;
    public const string SupervisorOrAdmin = Supervisor + "," + Admin;
    public const string SupervisorOrCheckerOrAdmin = Supervisor + "," + Checker + "," + Admin;
    public const string AppraiserOrSupervisorOrAdmin = Appraiser + "," + Supervisor + "," + Admin;
    public const string All = Appraiser + "," + Supervisor + "," + Checker + "," + Admin;
}
