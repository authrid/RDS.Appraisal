using Microsoft.AspNetCore.Components.Routing;

namespace AppraisalSystem.Web.Components.Layout;

public sealed record SecondarySidebarItem
{
	public SecondarySidebarItem(
		string title,
		string? href = null,
		NavLinkMatch match = NavLinkMatch.All,
		IReadOnlyList<SecondarySidebarItem>? children = null,
		bool isVisible = true)
	{
		Title = title;
		Href = href;
		Match = match;
		Children = children ?? [];
		IsVisible = isVisible;
	}

	public string Title { get; init; }

	public string? Href { get; init; }

	public NavLinkMatch Match { get; init; }

	public IReadOnlyList<SecondarySidebarItem> Children { get; init; }

	public bool IsVisible { get; init; }

	public IReadOnlyList<SecondarySidebarItem> VisibleChildren => Children.Where(x => x.IsVisible).ToList();

	public bool HasChildren => VisibleChildren.Count > 0;
}