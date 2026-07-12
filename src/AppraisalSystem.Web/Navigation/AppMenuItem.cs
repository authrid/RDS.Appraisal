using Microsoft.AspNetCore.Components.Routing;

namespace AppraisalSystem.Web.Navigation;

public sealed record AppMenuItem(
    string Key,
    string Title,
    string Href,
    AppMenuIcon Icon,
    NavLinkMatch Match,
    string? Roles = null);
