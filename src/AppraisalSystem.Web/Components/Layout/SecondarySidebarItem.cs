using Microsoft.AspNetCore.Components.Routing;

namespace AppraisalSystem.Web.Components.Layout;

public sealed record SecondarySidebarItem
{
	public SecondarySidebarItem(
		string title,
		string? href = null,
		NavLinkMatch match = NavLinkMatch.All,
		IReadOnlyList<SecondarySidebarItem>? children = null)
	{
		Title = title;
		Href = href;
		Match = match;
		Children = children ?? [];
	}

	public string Title { get; init; }

	public string? Href { get; init; }

	public NavLinkMatch Match { get; init; }

	public IReadOnlyList<SecondarySidebarItem> Children { get; init; }

	public bool HasChildren => Children.Count > 0;
}