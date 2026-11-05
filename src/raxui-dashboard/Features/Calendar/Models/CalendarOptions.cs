namespace raxui_dashboard.Features.Calendar.Models;

public static class CalendarOptions
{
    public static IReadOnlyList<string> Kinds { get; } = ["meeting", "review", "call", "event", "launch"];
}
