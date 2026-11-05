namespace raxui_dashboard.Features.Crm.Models;

public static class CrmOptions
{
    public static IReadOnlyList<string> Stages { get; } = ["Discovery", "Qualified", "Proposal sent", "Negotiation", "Won"];
}
