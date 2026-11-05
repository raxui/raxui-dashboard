namespace raxui_dashboard.Features.Customers.Models;

public static class CustomerOptions
{
    public static IReadOnlyList<string> Countries { get; } = ["Iraq", "UAE", "Germany", "United States", "Egypt", "France", "Jordan", "United Kingdom"];
    public static IReadOnlyList<string> PlanNames { get; } = ["Free", "Pro", "Business", "Enterprise"];
    public static IReadOnlyList<string> StatusNames { get; } = ["Active", "Trial", "Paused", "Churned"];
}
