using raxui_dashboard.Features.Calendar.Models;
using raxui_dashboard.Services.Localization;

namespace raxui_dashboard.Features.Calendar.Components;

public static class CalendarPresentation
{
    public static string Title(string kind) => char.ToUpperInvariant(kind[0]) + kind[1..];
    public static string Time(CalendarEvent value, Ui language) => value.Start == TimeOnly.MinValue ? language["All day"] : value.Start.ToString("HH:mm", language.Culture);
    public static string StatusOf(CalendarEvent value, DateOnly today, TimeOnly now) =>
        value.Date < today || value.Date == today && value.End <= now ? "done"
        : value.Date == today && value.Start <= now ? "current" : "upcoming";
    public static string ToneOf(string kind) => kind switch
    {
        "meeting" => "primary", "review" => "info", "call" => "success", "event" => "warning", _ => "danger",
    };
}
