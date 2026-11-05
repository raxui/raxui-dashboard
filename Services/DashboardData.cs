namespace raxui_dashboard.Services;

public sealed record Kpi(string Title, decimal Value, string Format, IReadOnlyList<double> History)
{
    /// <summary>Change of the last month against the one before, in percent.</summary>
    public double Trend => History[^2] == 0 ? 0 : Math.Round((History[^1] - History[^2]) / History[^2] * 100, 1);
}

public sealed record Order(string Id, string Customer, string Email, string Channel, DateOnly Date, string Status, decimal Amount);

public sealed record Customer(string Id, string Name, string Email, string Country, string Plan, string Status, int Orders, decimal Spent, DateOnly LastOrder);

public sealed record TaskCard(string Id, string Title, string Tag, string Priority, string Assignee, DateOnly? Due, int Comments);

public sealed record CalendarEvent(string Title, DateOnly Date, TimeOnly Start, TimeOnly End, string Kind);

public sealed record Overview(
    IReadOnlyList<DateOnly> Months,
    IReadOnlyList<Kpi> Kpis,
    IReadOnlyList<double> Revenue,
    IReadOnlyList<double> RevenueLastYear,
    IReadOnlyList<double> NewCustomers,
    IReadOnlyList<double> ReturningCustomers,
    IReadOnlyList<string> Channels,
    IReadOnlyList<double> ChannelShare,
    IReadOnlyList<Order> RecentOrders);

/// <summary>In-memory mock data for the showcase. Figures are consistent: orders = new + returning, AOV = revenue / orders.</summary>
public sealed class DashboardData
{
    private static readonly DateOnly LastMonth = new(2026, 10, 1);

    // Revenue in thousands of dollars, November 2025 to October 2026.
    private static readonly double[] Revenue = [42.6, 47.1, 45.3, 52.8, 58.2, 61.4, 59.7, 66.3, 72.1, 78.4, 84.2, 91.2];
    private static readonly double[] RevenueLastYear = [36.2, 39.4, 41.0, 40.3, 46.1, 48.5, 51.2, 50.4, 57.3, 60.1, 63.8, 70.4];
    private static readonly double[] NewCustomers = [182, 214, 196, 231, 258, 244, 276, 301, 288, 322, 345, 370];
    private static readonly double[] Returning = [121, 133, 149, 162, 171, 188, 202, 214, 229, 241, 262, 280];

    public Task<Overview> GetOverviewAsync()
    {
        var months = Enumerable.Range(0, 12).Select(i => LastMonth.AddMonths(i - 11)).ToArray();
        var orders = NewCustomers.Zip(Returning, (n, r) => n + r).ToArray();
        var aov = Revenue.Zip(orders, (rev, o) => Math.Round(rev * 1000 / o, 2)).ToArray();

        Kpi[] kpis =
        [
            new("Revenue", (decimal)Revenue[^1] * 1000, "money", Revenue),
            new("Orders", (decimal)orders[^1], "number", orders),
            new("New customers", (decimal)NewCustomers[^1], "number", NewCustomers),
            new("Avg. order value", (decimal)aov[^1], "money2", aov),
        ];

        Order[] recent =
        [
            new("#10482", "Layla Nasser", "layla.nasser@mail.com", "Online store", new(2026, 11, 5), "Paid", 249.00m),
            new("#10481", "Liam Carter", "liam.carter@mail.com", "Mobile app", new(2026, 11, 5), "Pending", 89.50m),
            new("#10480", "Omar Haddad", "omar.haddad@mail.com", "Marketplace", new(2026, 11, 4), "Shipped", 512.75m),
            new("#10479", "Sofia Martins", "sofia.martins@mail.com", "Online store", new(2026, 11, 4), "Paid", 134.20m),
            new("#10478", "Yusuf Rahman", "yusuf.rahman@mail.com", "Retail", new(2026, 11, 3), "Refunded", 64.00m),
            new("#10477", "Emma Schmidt", "emma.schmidt@mail.com", "Mobile app", new(2026, 11, 3), "Shipped", 318.40m),
            new("#10476", "Daniel Kim", "daniel.kim@mail.com", "Online store", new(2026, 11, 2), "Cancelled", 77.99m),
        ];

        return Task.FromResult(new Overview(
            months, kpis, Revenue, RevenueLastYear, NewCustomers, Returning,
            ["Online store", "Mobile app", "Marketplace", "Retail"], [46, 28, 16, 10], recent));
    }

    private static readonly string[] Names =
    [
        "Layla Nasser", "Liam Carter", "Omar Haddad", "Sofia Martins", "Yusuf Rahman", "Emma Schmidt", "Daniel Kim",
        "Noura Al-Qahtani", "Hassan Ali", "Chloe Dubois", "Mateo Rossi", "Aisha Karim", "Lucas Silva", "Reem Saleh",
        "Ethan Brooks", "Mariam Youssef", "Hiro Tanaka", "Zainab Farouk", "Oliver Grant", "Dana Mansour",
        "Ivan Petrov", "Salma Aziz", "Noah Fischer", "Fatima Zahra", "Leo Moreau", "Khalid Hamdan", "Ava Turner",
        "Sami Haddad", "Mia Jensen", "Tariq Aboud", "Grace Lee", "Rania Khoury", "Jonas Berg", "Huda Saeed",
        "Adam Novak", "Lina Sabri",
    ];
    public static readonly string[] Countries = ["Iraq", "UAE", "Germany", "United States", "Egypt", "France", "Jordan", "United Kingdom"];
    private static readonly string[] Plans = ["Free", "Pro", "Pro", "Business", "Enterprise"];
    private static readonly string[] Statuses = ["Active", "Active", "Active", "Trial", "Active", "Churned", "Active", "Paused"];

    // shortcut: data is in memory; this delay stands in for a network save so forms can show their loading state. Replace with a real store.
    public Task SaveAsync() => Task.Delay(900);

    public Task<IReadOnlyList<Customer>> GetCustomersAsync()
    {
        IReadOnlyList<Customer> customers = Names.Select((name, i) =>
        {
            var orders = 1 + i * 37 % 48;
            return new Customer(
                $"C-{2041 - i}", name,
                name.ToLowerInvariant().Replace(' ', '.').Replace("-", "") + "@mail.com",
                Countries[i * 3 % Countries.Length], Plans[i * 7 % Plans.Length], Statuses[i * 5 % Statuses.Length],
                orders, Math.Round(orders * (60m + i * 53 % 140), 2), new DateOnly(2026, 11, 5).AddDays(-(i * 11 % 60)));
        }).ToArray();
        return Task.FromResult(customers);
    }

    public IReadOnlyList<(string Column, TaskCard[] Cards)> GetTaskBoard() =>
    [
        ("backlog", [
            new("t1", "Customer segments report", "Analytics", "Low", "Hassan Ali", null, 0),
            new("t2", "Localize checkout emails to Arabic", "Content", "Medium", "Reem Saleh", new(2026, 11, 20), 2),
            new("t3", "Evaluate new payment provider", "Finance", "Low", "Omar Haddad", null, 1),
        ]),
        ("todo", [
            new("t4", "Black Friday landing page", "Marketing", "High", "Layla Nasser", new(2026, 11, 14), 5),
            new("t5", "Restock forecast for Q1", "Operations", "Medium", "Daniel Kim", new(2026, 11, 18), 0),
            new("t6", "Refund policy update", "Support", "Medium", "Sofia Martins", new(2026, 11, 12), 3),
        ]),
        ("doing", [
            new("t7", "Mobile app onboarding redesign", "Design", "High", "Noura Al-Qahtani", new(2026, 11, 10), 8),
            new("t8", "Migrate orders API to .NET 10", "Engineering", "High", "Yusuf Rahman", new(2026, 11, 9), 4),
        ]),
        ("done", [
            new("t9", "October revenue review", "Finance", "Medium", "Emma Schmidt", new(2026, 11, 3), 2),
            new("t10", "Dark mode for the storefront", "Design", "Low", "Liam Carter", new(2026, 11, 1), 6),
        ]),
    ];

    public IReadOnlyList<CalendarEvent> GetEvents() =>
    [
        new("Weekly sales sync", new(2026, 11, 2), new(9, 30), new(10, 0), "meeting"),
        new("October revenue review", new(2026, 11, 3), new(13, 0), new(14, 0), "review"),
        new("Supplier call: Gulf Logistics", new(2026, 11, 4), new(11, 0), new(11, 30), "call"),
        new("Weekly sales sync", new(2026, 11, 9), new(9, 30), new(10, 0), "meeting"),
        new("Product photo shoot", new(2026, 11, 5), new(10, 0), new(12, 30), "event"),
        new("Design review: onboarding", new(2026, 11, 5), new(14, 0), new(15, 0), "review"),
        new("1:1 with Yusuf", new(2026, 11, 5), new(16, 0), new(16, 30), "meeting"),
        new("Orders API go-live", new(2026, 11, 9), new(8, 0), new(9, 0), "launch"),
        new("Mobile onboarding handoff", new(2026, 11, 10), new(15, 0), new(16, 0), "review"),
        new("Refund policy published", new(2026, 11, 12), new(12, 0), new(12, 30), "launch"),
        new("Black Friday launch", new(2026, 11, 14), new(0, 0), new(1, 0), "launch"),
        new("Weekly sales sync", new(2026, 11, 16), new(9, 30), new(10, 0), "meeting"),
        new("Q1 forecast review", new(2026, 11, 18), new(13, 0), new(14, 30), "review"),
        new("Weekly sales sync", new(2026, 11, 23), new(9, 30), new(10, 0), "meeting"),
        new("Cyber Monday", new(2026, 11, 30), new(0, 0), new(1, 0), "launch"),
    ];
}
