using AppraisalSystem.Application.Common;
using Microsoft.AspNetCore.Components.Routing;

namespace AppraisalSystem.Web.Navigation;

// Single source of truth untuk struktur menu primer.
// AppSidebar melakukan render; MainLayout dapat memakai ResolveTitleForRoute untuk header.
public static class AppNavigation
{
    public static readonly IReadOnlyList<AppMenuItem> PrimaryMenu =
    [
        new("dashboard", "Dashboard", AppRoutes.Dashboard.Main, AppMenuIcon.Dashboard, NavLinkMatch.All),
        new("pencarian-data", "Pencarian Data", AppRoutes.PencarianData.Main, AppMenuIcon.Appraisal, NavLinkMatch.Prefix),
        new("pengkinian-data", "Pengkinian Data", AppRoutes.PengkinianData.Main, AppMenuIcon.Updating, NavLinkMatch.All, AppRoles.AppraiserOrAdmin),
        new("inquiry", "Inquiry", AppRoutes.Inquiry.Main, AppMenuIcon.Search, NavLinkMatch.All),
        new("report", "Report", AppRoutes.Report.Main, AppMenuIcon.Report, NavLinkMatch.Prefix),
        new("theme-preview", "Theme Preview", AppRoutes.SystemPages.ThemePreview, AppMenuIcon.Report, NavLinkMatch.All, AppRoles.Admin)
    ];

    // Cari label modul untuk breadcrumb/header berdasarkan segment pertama URL.
    public static string? ResolveTitleForFirstSegment(string? firstSegment)
    {
        if (string.IsNullOrWhiteSpace(firstSegment))
        {
            return null;
        }

        foreach (var item in PrimaryMenu)
        {
            var itemSegment = item.Href.TrimStart('/');
            if (string.Equals(itemSegment, firstSegment, StringComparison.OrdinalIgnoreCase))
            {
                return item.Title;
            }
        }

        return null;
    }
}
