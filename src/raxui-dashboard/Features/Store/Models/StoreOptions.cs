namespace raxui_dashboard.Features.Store.Models;

public static class StoreOptions
{
    public static IReadOnlyList<string> Periods { get; } = ["7d", "30d", "90d", "ytd"];
    public static IReadOnlyList<string> Channels { get; } = ["Online store", "Mobile app", "Marketplace", "Retail"];
}
