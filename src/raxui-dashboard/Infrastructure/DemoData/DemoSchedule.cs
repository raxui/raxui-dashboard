namespace raxui_dashboard.Infrastructure.DemoData;

/// <summary>The calendar, reports, and demo assistant share the sample data's reference day.</summary>
public static class DemoSchedule
{
    public static DateOnly Today => DashboardData.StoreToday;
    public static TimeOnly Now { get; } = new(14, 20);
}
