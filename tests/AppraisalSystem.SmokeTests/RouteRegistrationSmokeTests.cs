using System.Reflection;
using AppraisalSystem.Web;
using AppraisalSystem.Web.Components.Pages;
using Microsoft.AspNetCore.Components;

namespace AppraisalSystem.SmokeTests;

public sealed class RouteRegistrationSmokeTests
{
    [Theory]
    [InlineData(typeof(Dashboard), AppRoutes.Dashboard.Main)]
    [InlineData(typeof(AppraisalSystem.Web.Components.Pages.PengkinianData.Main), AppRoutes.PengkinianData.Main)]
    [InlineData(typeof(AppraisalSystem.Web.Components.Pages.Inquiry.Main), AppRoutes.Inquiry.Main)]
    [InlineData(typeof(AppraisalSystem.Web.Components.Pages.PencarianData.Main), AppRoutes.PencarianData.LegacyMain)]
    [InlineData(typeof(AppraisalSystem.Web.Components.Pages.Report.Main), AppRoutes.Report.Main)]
    [InlineData(typeof(AppraisalSystem.Web.Components.Pages.Report.Export), AppRoutes.Report.Export)]
    [InlineData(typeof(AppraisalSystem.Web.Components.Pages.ThemePreview), AppRoutes.SystemPages.ThemePreview)]
    public void FeatureRoutes_AreRegistered(Type componentType, string expectedRoute)
    {
        var routeTemplates = componentType
            .GetCustomAttributes<RouteAttribute>(inherit: false)
            .Select(x => x.Template)
            .ToArray();

        Assert.Contains(expectedRoute, routeTemplates, StringComparer.OrdinalIgnoreCase);
    }
}
