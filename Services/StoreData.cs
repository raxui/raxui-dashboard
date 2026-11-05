namespace raxui_dashboard.Services;

/// <summary>A headline figure for the period with the same figure for the period before it.</summary>
public sealed record StoreKpi(string Title, double Value, double Previous, string Format, bool LowerIsBetter = false)
{
    public double Change => Previous == 0 ? 0 : Math.Round((Value - Previous) / Previous * 100, 1);
    public bool Good => LowerIsBetter ? Change <= 0 : Change >= 0;
}

public sealed record ProductSale(string Name, string Category, int Units, double Revenue);

public sealed record StockItem(string Name, int Units, int Threshold);

public sealed record StoreView(
    string Period,
    string Bucket,                              // day, week or month
    IReadOnlyList<DateOnly> Buckets,
    IReadOnlyList<StoreKpi> Kpis,
    IReadOnlyList<double> Sales,
    IReadOnlyList<double> Visits,
    IReadOnlyList<(string Source, double Visits)> Sources,
    IReadOnlyList<ProductSale> TopProducts,
    IReadOnlyList<(string Category, double Share)> Categories,
    (int InStock, int Low, int Out) Stock,
    IReadOnlyList<StockItem> LowStock,
    double Rating, int Reviews, IReadOnlyList<int> RatingCounts,   // RatingCounts[0] = 5 stars
    IReadOnlyList<string> Reviewers,
    IReadOnlyList<Order> Orders);

/// <summary>
/// Store dashboard data. One deterministic daily model (1 Jan 2025 to 5 Nov 2026) is summed into the chosen period,
/// so every figure on the page agrees: orders x AOV = sales, visits x conversion = orders, and the previous period
/// is the same model one window earlier.
/// </summary>
public sealed partial class DashboardData
{
    public static readonly DateOnly StoreToday = new(2026, 11, 5);
    private static readonly DateOnly ModelStart = new(2025, 1, 1);
    public static readonly string[] Periods = ["7d", "30d", "90d", "ytd"];
    public static readonly string[] Channels = ["Online store", "Mobile app", "Marketplace", "Retail"];

    private sealed record Day(DateOnly Date, double Sales, double Orders, double Visits, double NewCustomers, double Returns);

    private static readonly Day[] Days = BuildDays();

    // Hash noise in [0, 1): stable across runs, no Random state.
    private static double Noise(int i, int salt)
    {
        var x = Math.Sin(i * 12.9898 + salt * 78.233) * 43758.5453;
        return x - Math.Floor(x);
    }

    private static Day[] BuildDays()
    {
        var count = StoreToday.DayNumber - ModelStart.DayNumber + 1;
        var days = new Day[count];
        for (var i = 0; i < count; i++)
        {
            var date = ModelStart.AddDays(i);
            var t = i / 365.0;
            var weekday = date.DayOfWeek switch
            {
                DayOfWeek.Thursday or DayOfWeek.Friday => 1.22, DayOfWeek.Saturday => 1.1, _ => 0.94,
            };
            var season = 1 + 0.12 * Math.Sin((date.DayOfYear - 80) / 365.0 * 2 * Math.PI);
            var sales = 1900 * (1 + 0.55 * t) * weekday * season * (0.86 + 0.28 * Noise(i, 1));
            var aov = 118 + 11 * t + 9 * Noise(i, 2);
            var orders = sales / aov;
            var conversion = 0.026 + 0.005 * t + 0.004 * Noise(i, 3);
            days[i] = new(date, sales, orders, orders / conversion, orders * (0.38 + 0.08 * Noise(i, 4)), orders * (0.018 + 0.01 * Noise(i, 5)));
        }
        return days;
    }

    // Channel share drifts over the model: the mobile app grows, retail shrinks.
    private static double Share(string channel, DateOnly date)
    {
        var t = (date.DayNumber - ModelStart.DayNumber) / 365.0;
        return channel switch
        {
            "Online store" => 0.47 - 0.01 * t,
            "Mobile app" => 0.24 + 0.05 * t,
            "Marketplace" => 0.17,
            "Retail" => 0.12 - 0.04 * t,
            _ => 1,
        };
    }

    private static (DateOnly From, DateOnly To) Window(string period, int back = 0)
    {
        if (period == "ytd")
        {
            var from = new DateOnly(StoreToday.Year - back, 1, 1);
            return (from, new DateOnly(StoreToday.Year - back, StoreToday.Month, StoreToday.Day));
        }
        var length = period switch { "7d" => 7, "30d" => 30, _ => 90 };
        var to = StoreToday.AddDays(-length * back);
        return (to.AddDays(1 - length), to);
    }

    private static Day[] Slice(DateOnly from, DateOnly to, string channel) =>
        Days.Where(d => d.Date >= from && d.Date <= to)
            .Select(d =>
            {
                var s = channel == "all" ? 1 : Share(channel, d.Date);
                return d with { Sales = d.Sales * s, Orders = d.Orders * s, Visits = d.Visits * s, NewCustomers = d.NewCustomers * s, Returns = d.Returns * s };
            })
            .ToArray();

    public Task<StoreView> GetStoreAsync(string period, string channel)
    {
        if (!Periods.Contains(period)) period = "30d";
        if (channel != "all" && !Channels.Contains(channel)) channel = "all";

        var (from, to) = Window(period);
        var (prevFrom, prevTo) = Window(period, 1);
        var now = Slice(from, to, channel);
        var before = Slice(prevFrom, prevTo, channel);

        var bucket = period switch { "7d" or "30d" => "day", "90d" => "week", _ => "month" };
        DateOnly Key(DateOnly d) => bucket switch
        {
            "day" => d,
            "week" => from.AddDays((d.DayNumber - from.DayNumber) / 7 * 7),
            _ => new DateOnly(d.Year, d.Month, 1),
        };
        var groups = now.GroupBy(d => Key(d.Date)).OrderBy(g => g.Key).ToArray();

        StoreKpi[] kpis =
        [
            new("Total sales", now.Sum(d => d.Sales), before.Sum(d => d.Sales), "money"),
            new("Orders", now.Sum(d => d.Orders), before.Sum(d => d.Orders), "number"),
            new("Avg. order value", now.Sum(d => d.Sales) / now.Sum(d => d.Orders), before.Sum(d => d.Sales) / before.Sum(d => d.Orders), "money2"),
            new("Conversion rate", now.Sum(d => d.Orders) / now.Sum(d => d.Visits) * 100, before.Sum(d => d.Orders) / before.Sum(d => d.Visits) * 100, "percent"),
            new("New customers", now.Sum(d => d.NewCustomers), before.Sum(d => d.NewCustomers), "number"),
            new("Return rate", now.Sum(d => d.Returns) / now.Sum(d => d.Orders) * 100, before.Sum(d => d.Returns) / before.Sum(d => d.Orders) * 100, "percent", LowerIsBetter: true),
        ];

        // Self-check: the chart buckets must add up to the headline figure.
        System.Diagnostics.Debug.Assert(Math.Abs(groups.Sum(g => g.Sum(d => d.Sales)) - kpis[0].Value) < 1, "Store buckets do not sum to total sales");

        var visits = now.Sum(d => d.Visits);
        (string, double)[] sourceShares = [("Organic search", 0.34), ("Direct", 0.22), ("Social", 0.18), ("Email", 0.11), ("Paid ads", 0.10), ("Referral", 0.05)];

        var sales = now.Sum(d => d.Sales);
        (string Name, string Category, double Share, double Price)[] products =
        [
            ("Linen overshirt", "Apparel", 0.14, 64), ("Everyday tote", "Accessories", 0.11, 38),
            ("Ceramic planter", "Home", 0.09, 29), ("Merino crew socks", "Apparel", 0.08, 16),
            ("Argan face oil", "Beauty", 0.07, 24), ("Leather card holder", "Accessories", 0.06, 32),
        ];

        // Reviews and stock scale with the period so a longer window shows more of them.
        var reviews = (int)Math.Round(now.Sum(d => d.Orders) * 0.19);
        double[] ratingShares = [0.71, 0.18, 0.06, 0.03, 0.02];

        return Task.FromResult(new StoreView(
            period, bucket,
            groups.Select(g => g.Key).ToArray(),
            kpis,
            groups.Select(g => Math.Round(g.Sum(d => d.Sales))).ToArray(),
            groups.Select(g => Math.Round(g.Sum(d => d.Visits))).ToArray(),
            sourceShares.Select(s => (s.Item1, Math.Round(visits * s.Item2))).ToArray(),
            products.Select(p => new ProductSale(p.Name, p.Category, (int)(sales * p.Share / p.Price), Math.Round(sales * p.Share))).ToArray(),
            [("Apparel", 41), ("Accessories", 27), ("Home", 19), ("Beauty", 13)],
            (1032, 148, 60),
            [new("Merino crew socks", 6, 40), new("Argan face oil", 11, 30), new("Linen overshirt", 18, 35)],
            4.6, reviews, ratingShares.Select(s => (int)Math.Round(reviews * s)).ToArray(),
            ["Layla Nasser", "Omar Haddad", "Sofia Martins", "Hassan Ali", "Emma Schmidt", "Yusuf Rahman", "Reem Saleh", "Daniel Kim"],
            RecentStoreOrders(channel)));
    }

    private static readonly string[] OrderStatuses = ["Paid", "Paid", "Shipped", "Pending", "Paid", "Shipped", "Refunded", "Paid", "Cancelled"];

    private static Order[] RecentStoreOrders(string channel)
    {
        var names = Names;
        var orders = Enumerable.Range(0, 40).Select(i =>
        {
            var name = names[i * 7 % names.Length];
            var roll = (int)(Noise(i, 9) * 100);
            var channelIndex = roll switch { < 46 => 0, < 74 => 1, < 90 => 2, _ => 3 };
            return new Order($"#{10482 - i}", name, name.ToLowerInvariant().Replace(' ', '.').Replace("-", "") + "@mail.com",
                Channels[channelIndex],
                StoreToday.AddDays(-(i / 4)), OrderStatuses[i * 5 % OrderStatuses.Length], Math.Round(24m + (decimal)(Noise(i, 10) * 480), 2));
        });
        return (channel == "all" ? orders : orders.Where(o => o.Channel == channel)).ToArray();
    }
}
