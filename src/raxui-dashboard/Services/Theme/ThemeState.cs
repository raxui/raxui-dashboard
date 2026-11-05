namespace raxui_dashboard.Services.Theme;

public sealed record ThemeState(string Appearance, string Accent, bool Dark, bool Rtl);

public static class ThemeOptions
{
    public static IReadOnlyList<string> Appearances { get; } = ["default", "flat", "glass"];
    public static IReadOnlyList<string> Accents { get; } = ["blue", "emerald", "indigo", "amber"];
    public static string Title(string value) => char.ToUpperInvariant(value[0]) + value[1..];
}
