namespace AppraisalSystem.Web.Options;

public sealed class ThemeOptions
{
    public const string SectionName = "Theme";

    public string BrandName { get; init; } = "RDS Appraisal";
    public string PrimaryColor { get; init; } = "#113a69";
    public string OnPrimaryColor { get; init; } = "#ffffff";
    public string PrimaryHoverColor { get; init; } = "#0c2f57";
    public string PrimarySoftColor { get; init; } = "#eef4ff";
    public string PrimarySoftBorderColor { get; init; } = "#bfd3f6";
    public string AccentColor { get; init; } = "#4f7ac4";

    public string? LogoPath { get; init; }
    public string? LoginHeroImageUrl { get; init; }
}

public static class ThemeOptionsValidator
{
    private static readonly System.Text.RegularExpressions.Regex HexColorRegex =
        new("^#([A-Fa-f0-9]{3}|[A-Fa-f0-9]{4}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$", System.Text.RegularExpressions.RegexOptions.Compiled);

    public static bool IsValid(ThemeOptions options)
    {
        return IsHex(options.PrimaryColor)
            && IsHex(options.OnPrimaryColor)
            && IsHex(options.PrimaryHoverColor)
            && IsHex(options.PrimarySoftColor)
            && IsHex(options.PrimarySoftBorderColor)
            && IsHex(options.AccentColor);
    }

    public static string NormalizeHexOrDefault(string? value, string fallback)
    {
        var candidate = value?.Trim();
        return IsHex(candidate) ? candidate! : fallback;
    }

    private static bool IsHex(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return HexColorRegex.IsMatch(value);
    }
}