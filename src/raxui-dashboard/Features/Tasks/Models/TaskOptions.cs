namespace raxui_dashboard.Features.Tasks.Models;

public static class TaskOptions
{
    public static readonly IReadOnlyDictionary<string, (string Title, int? Limit)> Meta = new Dictionary<string, (string Title, int? Limit)>()
    {
        ["backlog"] = ("Backlog", null), ["todo"] = ("To do", null), ["doing"] = ("In progress", 3), ["done"] = ("Done", null),
    };

    public static IReadOnlyList<string> Tags { get; } = ["Design", "Engineering", "Marketing", "Finance", "Operations", "Content", "Support", "Analytics"];
    public static IReadOnlyList<string> Priorities { get; } = ["Low", "Medium", "High"];
    public static IReadOnlyList<string> Team { get; } =
        ["Noura Al-Qahtani", "Layla Nasser", "Liam Carter", "Omar Haddad", "Sofia Martins", "Yusuf Rahman", "Emma Schmidt", "Daniel Kim"];
}
