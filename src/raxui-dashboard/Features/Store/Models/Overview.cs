using raxui_dashboard.Models.Reporting;

namespace raxui_dashboard.Features.Store.Models;

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
