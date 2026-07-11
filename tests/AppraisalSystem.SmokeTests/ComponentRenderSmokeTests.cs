using Bunit;
using AppraisalSystem.Web.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace AppraisalSystem.SmokeTests;

public sealed class ComponentRenderSmokeTests : TestContext
{
    [Fact]
    public void BaseCard_RendersTitleSubtitleAndChildContent()
    {
        var cut = RenderComponent<AppraisalSystem.Web.Components.Shared.Base.BaseCard>(parameters => parameters
            .Add(x => x.Title, "Smoke Card")
            .Add(x => x.Subtitle, "Render baseline")
            .AddChildContent("Card body content"));

        Assert.Contains("Smoke Card", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Render baseline", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Card body content", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BaseAlert_RendersVariantAndMessage()
    {
        var cut = RenderComponent<AppraisalSystem.Web.Components.Shared.Feedback.BaseAlert>(parameters => parameters
            .Add(x => x.Variant, "danger")
            .AddChildContent("Smoke alert message"));

        Assert.Contains("Smoke alert message", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("border-rose-200", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BaseButton_RendersLabelAndVariantClass()
    {
        var cut = RenderComponent<AppraisalSystem.Web.Components.Shared.Base.BaseButton>(parameters => parameters
            .Add(x => x.Variant, "danger")
            .AddChildContent("Delete"));

        Assert.Contains("Delete", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("border-rose-600", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AppSecondarySidebar_RendersNestedTree_WhenActiveChildRouteMatches()
    {
        const string publicId = "11111111-2222-3333-4444-555555555555";

        var items = new List<SecondarySidebarItem>
        {
            new(
                title: "Main",
                href: $"/pencarian-data/{publicId}",
                match: Microsoft.AspNetCore.Components.Routing.NavLinkMatch.All,
                children:
                [
                    new SecondarySidebarItem("Menu 1", "/not-found", Microsoft.AspNetCore.Components.Routing.NavLinkMatch.All),
                    new SecondarySidebarItem("Menu 2", "/not-found", Microsoft.AspNetCore.Components.Routing.NavLinkMatch.All)
                ]),
            new SecondarySidebarItem("Memo", $"/pencarian-data/memo?selectedPublicId={publicId}", Microsoft.AspNetCore.Components.Routing.NavLinkMatch.All),
            new SecondarySidebarItem("History", $"/pencarian-data/history?selectedPublicId={publicId}", Microsoft.AspNetCore.Components.Routing.NavLinkMatch.All)
        };

        var cut = RenderComponent<AppSecondarySidebar>(parameters => parameters
            .Add(x => x.Hidden, false)
            .Add(x => x.OffsetForPrimaryCompact, true)
            .Add(x => x.SectionLabel, "Sidebar 2")
            .Add(x => x.Title, "Pencarian Data")
            .Add(x => x.Items, items));

        Assert.Contains("Main", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Memo", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("History", cut.Markup, StringComparison.OrdinalIgnoreCase);
        cut.Find("button[aria-label='Open Main']").Click();
        Assert.Contains("Menu 1", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Menu 2", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }
}
