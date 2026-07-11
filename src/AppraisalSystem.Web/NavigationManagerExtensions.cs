using Microsoft.AspNetCore.Components;

namespace AppraisalSystem.Web;

public static class NavigationManagerExtensions
{
    public static string ToAppPath(this NavigationManager navigationManager, string route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return route;
        }

        if (Uri.TryCreate(route, UriKind.Absolute, out _)
            || route.StartsWith("//", StringComparison.Ordinal)
            || route.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return route;
        }

        var normalizedRoute = route.StartsWith("/", StringComparison.Ordinal) ? route[1..] : route;
        return new Uri(new Uri(navigationManager.BaseUri), normalizedRoute).PathAndQuery;
    }
}
