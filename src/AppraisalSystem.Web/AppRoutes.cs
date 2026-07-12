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

        public static string Detail(string publicId)
        {
            var normalized = TryNormalizePublicId(publicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? Main
                : $"/inquiry/{normalized}";
        }

        public static string MemoByPublicId(string publicId)
        {
            var normalized = TryNormalizePublicId(publicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? $"{Main}/memo"
                : $"/inquiry/{normalized}/memo";
        }

        public static string HistoryByPublicId(string publicId)
        {
            var normalized = TryNormalizePublicId(publicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? $"{Main}/history"
                : $"/inquiry/{normalized}/history";
        }

        private static string? TryNormalizePublicId(string? publicId)
        {
            if (!Guid.TryParse(publicId, out var parsed))
            {
                return null;
            }

            return parsed.ToString("D");
        }
    }

    public static string WithReadonly(string route, bool isReadonly)
    {
        if (!isReadonly)
        {
            return route;
        }

        return route.Contains('?', StringComparison.Ordinal) ? $"{route}&readonly=true" : $"{route}?readonly=true";
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

        public const string SelectedPublicIdQueryKey = "selectedPublicId";

        public static string Detail(string publicId)
        {
            var normalized = TryNormalizePublicId(publicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? Main
                : $"/pencarian-data/{normalized}";
        }

        public static string Edit(string publicId)
        {
            var normalized = TryNormalizePublicId(publicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? Main
                : $"/pencarian-data/{normalized}/edit";
        }

        public static string MemoByPublicId(string publicId)
        {
            var normalized = TryNormalizePublicId(publicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? Memo
                : $"/pencarian-data/{normalized}/memo";
        }

        public static string HistoryByPublicId(string publicId)
        {
            var normalized = TryNormalizePublicId(publicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? History
                : $"/pencarian-data/{normalized}/history";
        }

        public static string WithSelectedPublicId(string selectedPublicId)
        {
            var normalized = TryNormalizePublicId(selectedPublicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? Main
                : $"{Main}?{SelectedPublicIdQueryKey}={normalized}";
        }

        public static string MemoWithSelectedPublicId(string selectedPublicId)
        {
            var normalized = TryNormalizePublicId(selectedPublicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? Memo
                : $"{Memo}?{SelectedPublicIdQueryKey}={normalized}";
        }

        public static string HistoryWithSelectedPublicId(string selectedPublicId)
        {
            var normalized = TryNormalizePublicId(selectedPublicId);
            return string.IsNullOrWhiteSpace(normalized)
                ? History
                : $"{History}?{SelectedPublicIdQueryKey}={normalized}";
        }

        public static bool IsModuleSegment(string? segment) =>
            string.Equals(segment, Segment, StringComparison.OrdinalIgnoreCase)
            || string.Equals(segment, LegacySegment, StringComparison.OrdinalIgnoreCase);

        public static bool IsWorkspaceQuerySegment(string? segment) =>
            string.Equals(segment, "main", StringComparison.OrdinalIgnoreCase)
            || string.Equals(segment, "memo", StringComparison.OrdinalIgnoreCase)
            || string.Equals(segment, "history", StringComparison.OrdinalIgnoreCase);

        private static string? TryNormalizePublicId(string? publicId)
        {
            if (!Guid.TryParse(publicId, out var parsed))
            {
                return null;
            }

            return parsed.ToString("D");
        }
    }
}