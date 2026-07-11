namespace AppraisalSystem.Web;

public static class AppRoutes
{
    public static class Dashboard
    {
        public const string Root = "/";
        public const string Main = "/dashboard";
    }

    public static class Account
    {
        public const string Login = "/login";
        public const string LoginPost = "/account/login";
        public const string LogoutPost = "/account/logout";
        public const string AccessDenied = "/access-denied";
        public const string NotFound = "/not-found";
    }

    public static class SystemPages
    {
        public const string Error = "/Error";
        public const string ThemePreview = "/theme-preview";
    }

    public static class Report
    {
        public const string Main = "/report";
        public const string LegacyMain = "/report/main";
        public const string Export = "/report/export";
    }

    public static class PengkinianData
    {
        public const string Main = "/pengkinian-data";
    }

    public static class Inquiry
    {
        public const string Main = "/inquiry";
    }

    public static class PencarianData
    {
        public const string Segment = "pencarian-data";
        public const string LegacySegment = "appraisals";
        public const string Main = "/pencarian-data";
        public const string LegacyMain = "/pencarian-data/main";
        public const string LegacyIndex = "/appraisals";
        public const string Create = "/pencarian-data/create";
        public const string LegacyCreate = "/appraisals/create";
        public const string Memo = "/pencarian-data/memo";
        public const string History = "/pencarian-data/history";

        public static string Detail(int id) => $"/pencarian-data/{id}";

        public static string Edit(int id) => $"/pencarian-data/{id}/edit";

        public static string LegacyDetail(int id) => $"/appraisals/{id}";

        public static string LegacyEdit(int id) => $"/appraisals/{id}/edit";

        public static string WithSelectedId(int selectedId) => $"{Main}?selectedId={selectedId}";

        public static string MemoWithSelectedId(int selectedId) => $"{Memo}?selectedId={selectedId}";

        public static string HistoryWithSelectedId(int selectedId) => $"{History}?selectedId={selectedId}";

        public static bool IsModuleSegment(string? segment) =>
            string.Equals(segment, Segment, StringComparison.OrdinalIgnoreCase)
            || string.Equals(segment, LegacySegment, StringComparison.OrdinalIgnoreCase);

        public static bool IsWorkspaceQuerySegment(string? segment) =>
            string.Equals(segment, "main", StringComparison.OrdinalIgnoreCase)
            || string.Equals(segment, "memo", StringComparison.OrdinalIgnoreCase)
            || string.Equals(segment, "history", StringComparison.OrdinalIgnoreCase);
    }
}