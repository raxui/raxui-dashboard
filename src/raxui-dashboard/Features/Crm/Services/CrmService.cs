using raxui_dashboard.Features.Crm.Models;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Features.Crm.Services;

public sealed class CrmService(DashboardData data)
{
    public IReadOnlyList<string> Ranges => DashboardData.CrmRanges;
    public DateOnly Today => DemoSchedule.Today;
    public string MeetingStatus(Meeting meeting) =>
        meeting.Date < Today || meeting.Date == Today && meeting.End <= DemoSchedule.Now ? "done"
        : meeting.Date == Today && meeting.Start <= DemoSchedule.Now ? "current" : "upcoming";
    public Task<CrmView> LoadAsync(string range) => data.GetCrmAsync(range);
}
