using System.ComponentModel.DataAnnotations;
using raxui_dashboard.Features.Profile.Services;
using raxui_dashboard.Features.Settings.Services;
using raxui_dashboard.Infrastructure.DemoData;
using raxui_dashboard.Services.Time;

namespace raxui_dashboard.Tests;

public class AccountFeatureTests
{
    [Fact]
    public async Task Profile_keeps_validation_and_uses_the_application_clock_for_save_status()
    {
        var clock = new AppClock(new FixedTimeProvider(new(2026, 11, 5, 9, 30, 0, TimeSpan.Zero)));
        var service = new ProfileService(new DashboardData(), clock);
        service.Draft.Name = "";
        await Assert.ThrowsAsync<ValidationException>(service.SaveAsync);
        Assert.Null(service.SavedAt);
        service.Draft.Name = "Updated profile";
        await service.SaveAsync();
        Assert.Equal(clock.Now, service.SavedAt);
        Assert.Equal("Updated profile", service.Draft.Name);
    }

    [Fact]
    public void Notification_choices_belong_to_one_settings_page_instance()
    {
        var first = new SettingsService();
        var second = new SettingsService();
        Assert.Equal(4, first.Notifications.Count);
        Assert.True(first.Notifications[0].On);
        first.Notifications[0].On = false;
        Assert.True(second.Notifications[0].On);
    }
}
