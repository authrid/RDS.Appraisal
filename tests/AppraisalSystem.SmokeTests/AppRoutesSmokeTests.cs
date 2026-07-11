using AppraisalSystem.Web;

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
        Assert.Equal($"/pencarian-data/{publicId}/memo", AppRoutes.PencarianData.MemoByPublicId(publicId));
        Assert.Equal($"/pencarian-data/{publicId}/history", AppRoutes.PencarianData.HistoryByPublicId(publicId));
        Assert.Equal($"/pencarian-data/{publicId}/edit", AppRoutes.PencarianData.Edit(publicId));
    }

    [Fact]
    public void PencarianData_WorkspaceRoutes_UseConsistentPathBasedShape()
    {
        const string selectedPublicId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee";

        var mainRoute = AppRoutes.PencarianData.Detail(selectedPublicId);
        var memoRoute = AppRoutes.PencarianData.MemoByPublicId(selectedPublicId);
        var historyRoute = AppRoutes.PencarianData.HistoryByPublicId(selectedPublicId);

        Assert.Equal($"/pencarian-data/{selectedPublicId}", mainRoute);
        Assert.Equal($"/pencarian-data/{selectedPublicId}/memo", memoRoute);
        Assert.Equal($"/pencarian-data/{selectedPublicId}/history", historyRoute);
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