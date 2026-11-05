namespace raxui_dashboard.Services.Time;

/// <summary>One configurable local clock for user-facing dates and testable scheduling.</summary>
public sealed class AppClock(TimeProvider timeProvider, string timeZoneId = "Asia/Baghdad")
{
    private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    public DateTimeOffset Now => TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), timeZone);
    public DateOnly Today => DateOnly.FromDateTime(Now.DateTime);
}
