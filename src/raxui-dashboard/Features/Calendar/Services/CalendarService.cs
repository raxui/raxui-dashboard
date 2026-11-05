using raxui_dashboard.Features.Calendar.Models;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Features.Calendar.Services;

public sealed class CalendarService(DashboardData data)
{
    public DateOnly Today => DemoSchedule.Today;
    public TimeOnly Now => DemoSchedule.Now;
    public IReadOnlyList<CalendarEvent> Events { get; } = data.GetEvents();
}
