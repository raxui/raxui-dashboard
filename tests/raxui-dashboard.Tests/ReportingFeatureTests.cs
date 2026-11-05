using raxui_dashboard.Features.Analytics.Services;
using raxui_dashboard.Features.Calendar.Components;
using raxui_dashboard.Features.Calendar.Services;
using raxui_dashboard.Features.Crm.Services;
using raxui_dashboard.Features.Store.Services;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Tests;

public class ReportingFeatureTests
{
    [Theory]
    [InlineData("7d")]
    [InlineData("30d")]
    [InlineData("90d")]
    public async Task Analytics_keeps_the_store_session_totals_for_each_range(string range)
    {
        var source = new DashboardData();
        var store = await new StoreService(source).LoadAsync(range, "all");
        var analytics = await new AnalyticsService(source).LoadAsync(range);
        Assert.Equal(range, analytics.Range);
        Assert.Equal(store.Visits.Sum(), analytics.Sessions.Sum(), 6);
        Assert.Equal(analytics.Buckets.Count, analytics.Sessions.Count);
        Assert.Equal(analytics.Buckets.Count, analytics.ConversionRate.Count);
        var expected = await source.GetAnalyticsAsync(range);
        Assert.Equal(expected.Countries, analytics.Countries);
    }

    [Fact]
    public async Task Store_channel_filter_keeps_orders_in_the_selected_channel()
    {
        var service = new StoreService(new DashboardData());
        var view = await service.LoadAsync("30d", "Online store");
        Assert.NotEmpty(view.Orders);
        Assert.All(view.Orders, order => Assert.Equal("Online store", order.Channel));
        Assert.Equal("30d", view.Period);
        Assert.Equal(view.Sales.Sum(), Math.Round(view.Kpis[0].Value), 6);
    }

    [Theory]
    [InlineData("quarter")]
    [InlineData("6m")]
    [InlineData("12m")]
    public async Task Crm_pipeline_and_opportunities_stay_consistent(string range)
    {
        var view = await new CrmService(new DashboardData()).LoadAsync(range);
        Assert.Equal(range, view.Range);
        Assert.Equal(view.Opportunities.Count, view.Stages.Sum(s => s.Count));
        Assert.Equal(view.Opportunities.Sum(o => o.Value), view.Stages.Sum(s => s.Value));
        Assert.Equal(view.Buckets.Count, view.Qualified.Count);
    }

    [Fact]
    public void Calendar_preserves_the_sample_day_and_timeline_states()
    {
        var service = new CalendarService(new DashboardData());
        Assert.Equal(new DateOnly(2026, 11, 5), service.Today);
        var statuses = service.Events.Where(e => e.Date == service.Today)
            .Select(e => CalendarPresentation.StatusOf(e, service.Today, service.Now)).ToArray();
        Assert.Contains("done", statuses);
        Assert.Contains("current", statuses);
        Assert.Contains("upcoming", statuses);
        Assert.Contains(service.Events, e => e.Date > service.Today);
    }

    [Fact]
    public void Analytics_realtime_seed_keeps_twenty_chronological_readings()
    {
        var service = new AnalyticsService(new DashboardData());
        var now = new DateTime(2026, 11, 5, 12, 0, 0, DateTimeKind.Utc);
        var readings = service.InitialReadings(now);
        Assert.Equal(20, readings.Count);
        Assert.Equal(service.ActiveVisitors(now.AddSeconds(-57)), readings[0]);
        Assert.Equal(service.ActiveVisitors(now), readings[^1]);
    }
}
