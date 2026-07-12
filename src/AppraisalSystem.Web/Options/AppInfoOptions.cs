namespace AppraisalSystem.Web.Options;

/// <summary>
/// Application metadata from configuration (appsettings). Prefer this over hardcoding version in UI.
/// </summary>
public sealed class AppInfoOptions
{
    public const string SectionName = "App";

    /// <summary>Product / application display name.</summary>
    public string Name { get; init; } = "RDS Appraisal";

    /// <summary>Semantic version string, e.g. 1.0.0 (with or without leading v).</summary>
    public string Version { get; init; } = "1.0.0";

    /// <summary>Optional short tagline under the version (sidebar / login footer).</summary>
    public string? Tagline { get; init; } = "Internal System";

    public string DisplayVersion
    {
        get
        {
            var raw = Version?.Trim();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "v0.0.0";
            }

            return raw.StartsWith('v') || raw.StartsWith('V')
                ? $"v{raw[1..]}"
                : $"v{raw}";
        }
    }
}

public static class AppInfoOptionsValidator
{
    public static bool IsValid(AppInfoOptions options)
    {
        return !string.IsNullOrWhiteSpace(options.Name)
            && !string.IsNullOrWhiteSpace(options.Version);
    }
}
