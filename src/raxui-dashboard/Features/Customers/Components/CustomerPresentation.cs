namespace raxui_dashboard.Features.Customers.Components;

public static class CustomerPresentation
{
    public static string StatusColor(string s) => s switch { "Active" => "success", "Trial" => "info", "Paused" => "warning", _ => "neutral" };
    public static string PlanColor(string p) => p switch { "Enterprise" => "primary", "Business" => "info", "Pro" => "success", _ => "neutral" };
}
