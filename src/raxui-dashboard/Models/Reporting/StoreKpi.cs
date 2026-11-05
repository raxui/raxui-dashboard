namespace raxui_dashboard.Models.Reporting;

public sealed record StoreKpi(string Title, double Value, double Previous, string Format, bool LowerIsBetter = false)
{
    public double Change => Previous == 0 ? 0 : Math.Round((Value - Previous) / Previous * 100, 1);
    public bool Good => LowerIsBetter ? Change <= 0 : Change >= 0;
}
