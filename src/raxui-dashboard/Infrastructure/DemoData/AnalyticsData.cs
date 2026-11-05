using raxui_dashboard.Models.Reporting;
using raxui_dashboard.Features.Store.Models;
using raxui_dashboard.Features.Crm.Models;
using raxui_dashboard.Features.Analytics.Models;
using raxui_dashboard.Features.Calendar.Models;

namespace raxui_dashboard.Infrastructure.DemoData;

/// <summary>Web analytics derived from the store's daily model, so sessions and conversions match the E-commerce page.</summary>
public sealed partial class DashboardData
{
    public static readonly string[] AnalyticsRanges = ["7d", "30d", "90d"];

    public Task<AnalyticsView> GetAnalyticsAsync(string range)
    {
        if (!AnalyticsRanges.Contains(range)) range = "30d";
        var (from, to) = Window(range);
        var (prevFrom, prevTo) = Window(range, 1);
        var now = Slice(from, to, "all");
        var before = Slice(prevFrom, prevTo, "all");

        // Per-day derived measures; the day's index salts the noise so each day differs but stays stable.
        static (double Visitors, double Pageviews, double Engaged, double NewShare, double Seconds) Derive(Day d)
        {
            var i = d.Date.DayNumber;
            return (d.Visits * (0.66 + 0.04 * Noise(i, 41)), d.Visits * (2.4 + 0.5 * Noise(i, 42)), d.Visits * (0.55 + 0.08 * Noise(i, 43)),
                0.58 + 0.08 * Noise(i, 44), 96 + 30 * Noise(i, 45));
        }

        double Sum(Day[] days, Func<Day, double> f) => days.Sum(f);
        StoreKpi[] kpis =
        [
            new("Unique visitors", Sum(now, d => Derive(d).Visitors), Sum(before, d => Derive(d).Visitors), "number"),
            new("Sessions", Sum(now, d => d.Visits), Sum(before, d => d.Visits), "number"),
            new("Pageviews", Sum(now, d => Derive(d).Pageviews), Sum(before, d => Derive(d).Pageviews), "number"),
            new("Engagement rate", Sum(now, d => Derive(d).Engaged) / Sum(now, d => d.Visits) * 100, Sum(before, d => Derive(d).Engaged) / Sum(before, d => d.Visits) * 100, "percent"),
            new("Conversion rate", Sum(now, d => d.Orders) / Sum(now, d => d.Visits) * 100, Sum(before, d => d.Orders) / Sum(before, d => d.Visits) * 100, "percent"),
        ];

        var bucket = range == "90d" ? "week" : "day";
        var groups = now.GroupBy(d => bucket == "week" ? from.AddDays((d.Date.DayNumber - from.DayNumber) / 7 * 7) : d.Date).OrderBy(g => g.Key).ToArray();

        var sessions = Sum(now, d => d.Visits);
        var pageviews = Sum(now, d => Derive(d).Pageviews);
        var orders = Sum(now, d => d.Orders);
        (string Path, double Share, int Seconds, double Bounce)[] pages =
        [
            ("/", 0.24, 52, 0.38), ("/collections/new-in", 0.14, 94, 0.29), ("/products/linen-overshirt", 0.09, 128, 0.22),
            ("/collections/home", 0.07, 81, 0.33), ("/cart", 0.06, 64, 0.18), ("/products/everyday-tote", 0.05, 117, 0.24),
            ("/blog/summer-edit", 0.04, 186, 0.47),
        ];

        return Task.FromResult(new AnalyticsView(
            range, bucket,
            groups.Select(g => g.Key).ToArray(),
            kpis,
            groups.Select(g => Math.Round(g.Sum(d => Derive(d).Visitors))).ToArray(),
            groups.Select(g => Math.Round(g.Sum(d => d.Visits))).ToArray(),
            groups.Select(g => Math.Round(g.Sum(d => Derive(d).Visitors * Derive(d).NewShare))).ToArray(),
            groups.Select(g => Math.Round(g.Sum(d => Derive(d).Visitors * (1 - Derive(d).NewShare)))).ToArray(),
            groups.Select(g => Math.Round(g.Average(d => Derive(d).Seconds))).ToArray(),
            groups.Select(g => Math.Round(g.Sum(d => d.Orders) / g.Sum(d => d.Visits) * 100, 2)).ToArray(),
            pages.Select(p => new PageStat(p.Path, Math.Round(pageviews * p.Share), p.Seconds, p.Bounce)).ToArray(),
            [("Organic search", Math.Round(sessions * 0.34)), ("Direct", Math.Round(sessions * 0.22)), ("Social", Math.Round(sessions * 0.18)),
             ("Email", Math.Round(sessions * 0.11)), ("Paid ads", Math.Round(sessions * 0.10)), ("Referral", Math.Round(sessions * 0.05))],
            [("Black Friday early access", Math.Round(sessions * 0.06), 5.8), ("Autumn collection", Math.Round(sessions * 0.04), 3.9),
             ("Weekly newsletter", Math.Round(sessions * 0.05), 4.6), ("Instagram reels", Math.Round(sessions * 0.07), 2.1)],
            [("instagram.com", Math.Round(sessions * 0.09)), ("google.com", Math.Round(sessions * 0.07)), ("tiktok.com", Math.Round(sessions * 0.05)),
             ("pinterest.com", Math.Round(sessions * 0.02)), ("reddit.com", Math.Round(sessions * 0.01))],
            [("Mobile", 64), ("Desktop", 29), ("Tablet", 7)],
            new (string, string, double)[] { ("IQ", "Iraq", 0.27), ("AE", "UAE", 0.18), ("JO", "Jordan", 0.12), ("EG", "Egypt", 0.11), ("KW", "Kuwait", 0.08),
             ("DE", "Germany", 0.07), ("US", "United States", 0.06), ("GB", "United Kingdom", 0.05) }
                .Select(c => (c.Item1, c.Item2, Math.Round(kpis[0].Value * c.Item3))).ToArray(),
            [("Sessions", sessions), ("Product views", Math.Round(sessions * 0.58)), ("Added to cart", Math.Round(sessions * 0.14)),
             ("Checkout", Math.Round(orders * 1.6)), ("Purchase", Math.Round(orders))]));
    }

    /// <summary>Visitors on the site in the last minute; a stable wave plus noise keyed to the second.</summary>
    public static int ActiveVisitors(DateTime utc)
    {
        var tick = (int)(utc.Ticks / TimeSpan.TicksPerSecond / 3);
        return (int)Math.Round(22 + 6 * Math.Sin(tick / 9.0) + 5 * Noise(tick, 51));
    }
}
