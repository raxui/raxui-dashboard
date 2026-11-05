using Microsoft.AspNetCore.Components;
using raxui_dashboard.Features.Analytics.Models;
using raxui_dashboard.Services.Localization;

namespace raxui_dashboard.Features.Analytics.Components;

public abstract class AnalyticsSectionBase : ComponentBase
{
    [CascadingParameter] public Ui L { get; set; } = new(false);
    [Parameter, EditorRequired] public AnalyticsView View { get; set; } = default!;
    protected AnalyticsView view => View;
    protected string[] BucketLabels => View.Buckets.Select(d => d.ToString("d MMM", L.Culture)).ToArray();
    protected string Percent(double part, double whole) => whole == 0 ? "0%" : (part / whole * 100).ToString("0", L.Culture) + "%";
    protected string Duration(int seconds) => $"{seconds / 60}{L["m"]} {seconds % 60}{L["s"]}";
    protected static string PreviousName(string range) => range switch { "7d" => "previous 7 days", "30d" => "previous 30 days", _ => "previous 90 days" };
}
