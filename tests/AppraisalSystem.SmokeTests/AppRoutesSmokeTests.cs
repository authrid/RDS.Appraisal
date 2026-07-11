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
        Assert.Equal("/pencarian-data", AppRoutes.PencarianData.Main);
        Assert.Equal("/pencarian-data/create", AppRoutes.PencarianData.Create);
        Assert.Equal("/pencarian-data/42", AppRoutes.PencarianData.Detail(42));
        Assert.Equal("/pencarian-data/42/edit", AppRoutes.PencarianData.Edit(42));
        Assert.Equal("/pencarian-data?selectedId=42", AppRoutes.PencarianData.WithSelectedId(42));
        Assert.Equal("/pencarian-data/memo?selectedId=42", AppRoutes.PencarianData.MemoWithSelectedId(42));
        Assert.Equal("/pencarian-data/history?selectedId=42", AppRoutes.PencarianData.HistoryWithSelectedId(42));
    }

    [Fact]
    public void PencarianData_WorkspaceRoutes_KeepSelectedIdAcrossMainMemoHistory()
    {
        const int selectedId = 77;

        var mainRoute = AppRoutes.PencarianData.WithSelectedId(selectedId);
        var memoRoute = AppRoutes.PencarianData.MemoWithSelectedId(selectedId);
        var historyRoute = AppRoutes.PencarianData.HistoryWithSelectedId(selectedId);

        Assert.Equal("77", QueryHelpers.ParseQuery(new Uri($"https://localhost{mainRoute}").Query)["selectedId"].ToString());
        Assert.Equal("77", QueryHelpers.ParseQuery(new Uri($"https://localhost{memoRoute}").Query)["selectedId"].ToString());
        Assert.Equal("77", QueryHelpers.ParseQuery(new Uri($"https://localhost{historyRoute}").Query)["selectedId"].ToString());
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