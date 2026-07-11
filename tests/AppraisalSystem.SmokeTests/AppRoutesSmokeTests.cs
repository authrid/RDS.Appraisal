using AppraisalSystem.Web;
using Microsoft.AspNetCore.WebUtilities;

namespace AppraisalSystem.SmokeTests;

public sealed class AppRoutesSmokeTests
{
    [Fact]
    public void AccountAndSystemRoutes_AreStable()
    {
        Assert.Equal("/login", AppRoutes.Account.Login);
        Assert.Equal("/account/login", AppRoutes.Account.LoginPost);
        Assert.Equal("/account/logout", AppRoutes.Account.LogoutPost);
        Assert.Equal("/access-denied", AppRoutes.Account.AccessDenied);
        Assert.Equal("/not-found", AppRoutes.Account.NotFound);
        Assert.Equal("/Error", AppRoutes.SystemPages.Error);
        Assert.Equal("/theme-preview", AppRoutes.SystemPages.ThemePreview);
    }

    [Fact]
    public void TopLevelModuleRoutes_AreStable()
    {
        Assert.Equal("/dashboard", AppRoutes.Dashboard.Main);
        Assert.Equal("/pengkinian-data", AppRoutes.PengkinianData.Main);
        Assert.Equal("/inquiry", AppRoutes.Inquiry.Main);
        Assert.Equal("/report", AppRoutes.Report.Main);
        Assert.Equal("/report/main", AppRoutes.Report.LegacyMain);
        Assert.Equal("/report/export", AppRoutes.Report.Export);
    }

    [Fact]
    public void PencarianData_Helpers_BuildCanonicalRoutes()
    {
        var publicId = Guid.Parse("11111111-2222-3333-4444-555555555555").ToString("D");

        Assert.Equal("/pencarian-data", AppRoutes.PencarianData.Main);
        Assert.Equal("/pencarian-data/create", AppRoutes.PencarianData.Create);
        Assert.Equal($"/pencarian-data/{publicId}", AppRoutes.PencarianData.Detail(publicId));
        Assert.Equal($"/pencarian-data/{publicId}/edit", AppRoutes.PencarianData.Edit(publicId));
        Assert.Equal($"/pencarian-data?selectedPublicId={publicId}", AppRoutes.PencarianData.WithSelectedPublicId(publicId));
        Assert.Equal($"/pencarian-data/memo?selectedPublicId={publicId}", AppRoutes.PencarianData.MemoWithSelectedPublicId(publicId));
        Assert.Equal($"/pencarian-data/history?selectedPublicId={publicId}", AppRoutes.PencarianData.HistoryWithSelectedPublicId(publicId));
    }

    [Fact]
    public void PencarianData_WorkspaceRoutes_KeepSelectedPublicIdAcrossMainMemoHistory()
    {
        const string selectedPublicId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee";

        var mainRoute = AppRoutes.PencarianData.WithSelectedPublicId(selectedPublicId);
        var memoRoute = AppRoutes.PencarianData.MemoWithSelectedPublicId(selectedPublicId);
        var historyRoute = AppRoutes.PencarianData.HistoryWithSelectedPublicId(selectedPublicId);

        Assert.Equal(selectedPublicId, QueryHelpers.ParseQuery(new Uri($"https://localhost{mainRoute}").Query)["selectedPublicId"].ToString());
        Assert.Equal(selectedPublicId, QueryHelpers.ParseQuery(new Uri($"https://localhost{memoRoute}").Query)["selectedPublicId"].ToString());
        Assert.Equal(selectedPublicId, QueryHelpers.ParseQuery(new Uri($"https://localhost{historyRoute}").Query)["selectedPublicId"].ToString());
    }

    [Theory]
    [InlineData("pencarian-data", true)]
    [InlineData("appraisals", true)]
    [InlineData("report", false)]
    [InlineData(null, false)]
    public void PencarianData_ModuleSegmentDetection_IsStable(string? segment, bool expected)
    {
        Assert.Equal(expected, AppRoutes.PencarianData.IsModuleSegment(segment));
    }

    [Theory]
    [InlineData("main", true)]
    [InlineData("memo", true)]
    [InlineData("history", true)]
    [InlineData("detail", false)]
    [InlineData(null, false)]
    public void PencarianData_WorkspaceQuerySegmentDetection_IsStable(string? segment, bool expected)
    {
        Assert.Equal(expected, AppRoutes.PencarianData.IsWorkspaceQuerySegment(segment));
    }
}