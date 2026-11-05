namespace raxui_dashboard.Features.Calendar.Models;

public sealed record CalendarEvent(string Title, DateOnly Date, TimeOnly Start, TimeOnly End, string Kind);
