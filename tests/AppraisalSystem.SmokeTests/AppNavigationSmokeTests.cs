using AppraisalSystem.Application.Common;
using AppraisalSystem.Web;
using AppraisalSystem.Web.Navigation;

namespace AppraisalSystem.SmokeTests;

public sealed class AppNavigationSmokeTests
{
    [Fact]
    public void PrimaryMenu_ContainsAllExpectedModules()
    {
        var keys = AppNavigation.PrimaryMenu.Select(x => x.Key).ToArray();

        Assert.Contains("dashboard", keys);
        Assert.Contains("pencarian-data", keys);
        Assert.Contains("pengkinian-data", keys);
        Assert.Contains("inquiry", keys);
        Assert.Contains("report", keys);
    }

    [Fact]
    public void PengkinianMenu_IsRestrictedToAppraiserAndAdmin()
    {
        var item = AppNavigation.PrimaryMenu.First(x => x.Key == "pengkinian-data");
        Assert.Equal(AppRoles.AppraiserOrAdmin, item.Roles);
    }

    [Fact]
    public void ThemePreviewMenu_IsRestrictedToAdmin()
    {
        var item = AppNavigation.PrimaryMenu.First(x => x.Key == "theme-preview");
        Assert.Equal(AppRoles.Admin, item.Roles);
    }

    [Theory]
    [InlineData("dashboard", "Dashboard")]
    [InlineData("pengkinian-data", "Pengkinian Data")]
    [InlineData("inquiry", "Inquiry")]
    [InlineData("report", "Report")]
    [InlineData("unknown-segment", null)]
    [InlineData("", null)]
    public void ResolveTitleForFirstSegment_MapsKnownSegments(string segment, string? expected)
    {
        Assert.Equal(expected, AppNavigation.ResolveTitleForFirstSegment(segment));
    }
}
