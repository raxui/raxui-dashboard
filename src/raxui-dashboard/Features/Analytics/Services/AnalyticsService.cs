using raxui_dashboard.Features.Analytics.Models;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Features.Analytics.Services;

public sealed class AnalyticsService(DashboardData data)
{
    public IReadOnlyList<string> Ranges => DashboardData.AnalyticsRanges;
    public Task<AnalyticsView> LoadAsync(string range) => data.GetAnalyticsAsync(range);
    public int ActiveVisitors(DateTime utcNow) => DashboardData.ActiveVisitors(utcNow);
    public IReadOnlyList<double> InitialReadings(DateTime utcNow) =>
        Enumerable.Range(0, 20).Select(i => (double)ActiveVisitors(utcNow.AddSeconds(-3 * (19 - i)))).ToArray();
}
