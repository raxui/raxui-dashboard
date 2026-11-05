namespace raxui_dashboard.Features.Crm.Models;

public sealed record Meeting(string Title, string Account, DateOnly Date, TimeOnly Start, TimeOnly End, string Kind);
