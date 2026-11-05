namespace raxui_dashboard.Features.Analytics.Models;

public sealed record PageStat(string Path, double Views, int AvgSeconds, double Bounce);
