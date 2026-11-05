using raxui_dashboard.Models.Reporting;

namespace raxui_dashboard.Features.Crm.Models;

public sealed record CrmView(
    string Range,
    string Bucket,                                  // week or month
    IReadOnlyList<DateOnly> Buckets,
    IReadOnlyList<StoreKpi> Kpis,
    IReadOnlyList<double> Qualified,
    IReadOnlyList<double> OtherLeads,
    int CallsBooked,
    IReadOnlyList<(string Stage, int Count, decimal Value)> Stages,
    IReadOnlyList<Meeting> Meetings,
    int ProposalsSent, int ProposalTarget, decimal WonThisMonth,
    IReadOnlyList<Opportunity> Opportunities);
