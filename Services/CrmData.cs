namespace raxui_dashboard.Services;

public sealed record Opportunity(string Id, string Account, string Contact, string Owner, string Stage, string Priority, string Health,
    int HealthScore, decimal Value, DateOnly Close);

public sealed record Meeting(string Title, string Account, DateOnly Date, TimeOnly Start, TimeOnly End, string Kind);

public sealed record CrmView(
    string Range,
    string Bucket,                                  // week or month
    IReadOnlyList<DateOnly> Buckets,
    IReadOnlyList<StoreKpi> Kpis,
    IReadOnlyList<double> Qualified,
    IReadOnlyList<double> OtherLeads,
    int CallsBooked,
    IReadOnlyList<(string Stage, int Count, decimal Value)> Stages,
    IReadOnlyList<Meeting> Meetings,
    int ProposalsSent, int ProposalTarget, decimal WonThisMonth,
    IReadOnlyList<Opportunity> Opportunities);

/// <summary>
/// CRM for the store's wholesale accounts. Leads come from one deterministic daily model (summed per range, the
/// previous range is the same model one window earlier); the pipeline, stage totals and table share one list.
/// </summary>
public sealed partial class DashboardData
{
    public static readonly string[] CrmRanges = ["quarter", "6m", "12m"];
    public static readonly string[] Stages = ["Discovery", "Qualified", "Proposal sent", "Negotiation", "Won"];
    public static readonly string[] HealthLevels = ["On track", "Needs review", "At risk", "On hold"];

    private sealed record LeadDay(DateOnly Date, double Leads, double Qualified, double Won, double Calls);

    // Lazy: static initializers in different partial files run in an unspecified order (Days, Names).
    private static LeadDay[]? leadDays;
    private static LeadDay[] LeadDays => leadDays ??= Days.Select((d, i) =>
    {
        var t = (d.Date.DayNumber - GrowthAnchor.DayNumber) / 365.0;
        var leads = 4.2 * (1 + 0.22 * t) * (d.Date.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday ? 0.45 : 1.15) * (0.7 + 0.6 * Noise(i, 21));
        var qualified = leads * (0.33 + 0.03 * t + 0.06 * Noise(i, 22));
        return new LeadDay(d.Date, leads, qualified, qualified * (0.27 + 0.05 * Noise(i, 23)), qualified * 0.36);
    }).ToArray();

    // Ranges end with the last full week (quarter) or the last full month, so no bar shows a partial bucket.
    private static (DateOnly From, DateOnly To) CrmWindow(string range, int back = 0)
    {
        var lastMonthEnd = new DateOnly(StoreToday.Year, StoreToday.Month, 1).AddDays(-1);
        return range switch
        {
            "quarter" => (StoreToday.AddDays(-91 * (back + 1) + 1), StoreToday.AddDays(-91 * back)),
            "6m" => (new DateOnly(lastMonthEnd.Year, lastMonthEnd.Month, 1).AddMonths(-5 - 6 * back), new DateOnly(lastMonthEnd.Year, lastMonthEnd.Month, 1).AddMonths(1 - 6 * back).AddDays(-1)),
            _ => (new DateOnly(lastMonthEnd.Year, lastMonthEnd.Month, 1).AddMonths(-11 - 12 * back), new DateOnly(lastMonthEnd.Year, lastMonthEnd.Month, 1).AddMonths(1 - 12 * back).AddDays(-1)),
        };
    }

    private static readonly string[] Accounts =
    [
        "Gulf Retail Group", "Baghdad Home Co.", "Nile Boutiques", "Amman Outfitters", "Riverside Hotels", "Cedar Spa & Wellness",
        "Atlas Concept Stores", "Najd Gifts", "Harbor Hospitality", "Oasis Fitness Clubs", "Tigris Office Supply", "Palm Events",
        "Basra Marine Club", "Levant Living", "Sahara Outdoor", "Euphrates Pharmacy", "Crescent Airlines", "Marina Residences",
        "Erbil Grand Mall", "Corniche Cafés", "Golden Sands Resort", "Al Noor Schools", "Desert Rose Florists", "Lumen Coworking",
    ];

    private static readonly string[] Owners = ["Noura Al-Qahtani", "Omar Haddad", "Layla Nasser", "Daniel Kim"];

    private static Opportunity[]? opportunities;
    private static Opportunity[] Opportunities => opportunities ??= Accounts.Select((account, i) =>
    {
        var stage = Stages[(int)(Noise(i, 31) * 100) switch { < 26 => 0, < 48 => 1, < 70 => 2, < 88 => 3, _ => 4 }];
        var score = stage == "Won" ? 10 : 2 + (int)(Noise(i, 32) * 9);
        var health = stage == "Won" ? "On track" : score switch { >= 7 => "On track", >= 5 => "Needs review", >= 3 => "At risk", _ => "On hold" };
        return new Opportunity($"OP-{1842 - i}", account, Names[(i * 5 + 3) % Names.Length], Owners[i % Owners.Length], stage,
            score >= 8 ? "Low" : score >= 5 ? "Medium" : "High", health, score,
            Math.Round((4000m + (decimal)(Noise(i, 33) * 76000)) / 100) * 100,
            StoreToday.AddDays(stage == "Won" ? -(int)(Noise(i, 34) * 4) : 7 + (int)(Noise(i, 34) * 80)));
    }).ToArray();

    public Task<CrmView> GetCrmAsync(string range)
    {
        if (!CrmRanges.Contains(range)) range = "12m";
        var (from, to) = CrmWindow(range);
        var (prevFrom, prevTo) = CrmWindow(range, 1);
        var now = LeadDays.Where(d => d.Date >= from && d.Date <= to).ToArray();
        var before = LeadDays.Where(d => d.Date >= prevFrom && d.Date <= prevTo).ToArray();

        var bucket = range == "quarter" ? "week" : "month";
        var groups = now.GroupBy(d => bucket == "week" ? from.AddDays((d.Date.DayNumber - from.DayNumber) / 7 * 7) : new DateOnly(d.Date.Year, d.Date.Month, 1))
            .OrderBy(g => g.Key).ToArray();

        var open = Opportunities.Where(o => o.Stage != "Won").ToArray();
        var pipeline = (double)open.Sum(o => o.Value);
        // The pipeline is a snapshot; scale it by qualified-lead growth to estimate where it stood a range ago.
        var growth = before.Sum(d => d.Qualified) / now.Sum(d => d.Qualified);

        StoreKpi[] kpis =
        [
            new("Pipeline value", pipeline, pipeline * growth, "money"),
            new("Qualified lead rate", now.Sum(d => d.Qualified) / now.Sum(d => d.Leads) * 100, before.Sum(d => d.Qualified) / before.Sum(d => d.Leads) * 100, "percent"),
            new("Open opportunities", open.Length, Math.Round(open.Length * growth), "number"),
            new("Lead-to-deal rate", now.Sum(d => d.Won) / now.Sum(d => d.Leads) * 100, before.Sum(d => d.Won) / before.Sum(d => d.Leads) * 100, "percent"),
        ];

        var today = StoreToday;
        Meeting[] meetings =
        [
            new("Discovery call", "Najd Gifts", today, new(9, 30), new(10, 0), "call"),
            new("Proposal review", "Riverside Hotels", today, new(13, 0), new(13, 45), "review"),
            new("Negotiation", "Gulf Retail Group", today, new(14, 0), new(15, 0), "meeting"),
            new("Product demo", "Lumen Coworking", today, new(16, 30), new(17, 0), "event"),
            new("Contract signing", "Erbil Grand Mall", today.AddDays(1), new(11, 0), new(11, 30), "launch"),
            new("Discovery call", "Corniche Cafés", today.AddDays(1), new(15, 0), new(15, 30), "call"),
        ];

        return Task.FromResult(new CrmView(
            range, bucket,
            groups.Select(g => g.Key).ToArray(),
            kpis,
            groups.Select(g => Math.Round(g.Sum(d => d.Qualified))).ToArray(),
            groups.Select(g => Math.Round(g.Sum(d => d.Leads - d.Qualified))).ToArray(),
            (int)Math.Round(now.Sum(d => d.Calls)),
            Stages.Select(s => (s, Opportunities.Count(o => o.Stage == s), Opportunities.Where(o => o.Stage == s).Sum(o => o.Value))).ToArray(),
            meetings,
            12, 18, Opportunities.Where(o => o.Stage == "Won" && o.Close.Month == today.Month).Sum(o => o.Value),
            Opportunities));
    }
}
