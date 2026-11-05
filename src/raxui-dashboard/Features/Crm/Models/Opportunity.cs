namespace raxui_dashboard.Features.Crm.Models;

public sealed record Opportunity(string Id, string Account, string Contact, string Owner, string Stage, string Priority, string Health,
    int HealthScore, decimal Value, DateOnly Close);
