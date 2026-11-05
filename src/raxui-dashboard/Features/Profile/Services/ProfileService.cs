using System.ComponentModel.DataAnnotations;
using raxui_dashboard.Features.Profile.Models;
using raxui_dashboard.Infrastructure.DemoData;
using raxui_dashboard.Services.Time;

namespace raxui_dashboard.Features.Profile.Services;

public sealed class ProfileService(DashboardData data, AppClock clock)
{
    public ProfileDraft Draft { get; } = new();
    public DateTimeOffset? SavedAt { get; private set; }
    public async Task SaveAsync()
    {
        Validator.ValidateObject(Draft, new ValidationContext(Draft), validateAllProperties: true);
        SavedAt = null;
        await data.SaveAsync();
        SavedAt = clock.Now;
    }
}
