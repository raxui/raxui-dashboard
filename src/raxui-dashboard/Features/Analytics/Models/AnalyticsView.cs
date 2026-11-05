using raxui_dashboard.Models.Reporting;

namespace raxui_dashboard.Features.Analytics.Models;

public sealed record AnalyticsView(
    string Range,
    string Bucket,                                   // day or week
    IReadOnlyList<DateOnly> Buckets,
    IReadOnlyList<StoreKpi> Kpis,
    IReadOnlyList<double> Visitors,
    IReadOnlyList<double> Sessions,
    IReadOnlyList<double> NewVisitors,
    IReadOnlyList<double> ReturningVisitors,
    IReadOnlyList<double> EngagementSeconds,
    IReadOnlyList<double> ConversionRate,
    IReadOnlyList<PageStat> Pages,
    IReadOnlyList<(string Name, double Visits)> Sources,
    IReadOnlyList<(string Name, double Visits, double Conversion)> Campaigns,
    IReadOnlyList<(string Name, double Visits)> Referrers,
    IReadOnlyList<(string Name, double Share)> Devices,
    IReadOnlyList<(string Code, string Name, double Visitors)> Countries,
    IReadOnlyList<(string Step, double Count)> Funnel);
