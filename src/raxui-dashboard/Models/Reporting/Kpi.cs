namespace raxui_dashboard.Models.Reporting;

public sealed record Kpi(string Title, decimal Value, string Format, IReadOnlyList<double> History)
{
    /// <summary>Change of the last month against the one before, in percent.</summary>
    public double Trend => History[^2] == 0 ? 0 : Math.Round((History[^1] - History[^2]) / History[^2] * 100, 1);
}
