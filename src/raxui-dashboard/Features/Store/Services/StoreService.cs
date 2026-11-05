using raxui_dashboard.Features.Store.Models;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Features.Store.Services;

public sealed class StoreService(DashboardData data)
{
    public DateOnly Today => DemoSchedule.Today;
    public Task<StoreView> LoadAsync(string period, string channel) => data.GetStoreAsync(period, channel);
}
