using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using raxui_dashboard.Features.Customers.Services;
using raxui_dashboard.Services;
using raxui_dashboard.Services.Localization;
using raxui_dashboard.Services.Theme;
using raxui_dashboard.Services.Time;

namespace raxui_dashboard.Tests;

public class ApplicationStateTests
{
    [Fact]
    public void Application_clock_uses_configured_zone_at_the_utc_date_boundary()
    {
        var provider = new FixedTimeProvider(new(2026, 11, 4, 22, 30, 0, TimeSpan.Zero));
        Assert.Equal(new DateOnly(2026, 11, 5), new AppClock(provider, "Asia/Baghdad").Today);
        Assert.Equal(new DateOnly(2026, 11, 4), new AppClock(provider, "UTC").Today);
    }

    [Fact]
    public async Task Theme_loads_once_and_notifies_when_language_changes()
    {
        var js = new ThemeJsRuntime();
        var service = new ThemeService(js);
        var changed = 0;
        service.Changed += () => changed++;
        await service.InitializeAsync();
        await service.InitializeAsync();
        Assert.Equal(1, js.LoadCount);
        var next = new ThemeState("flat", "emerald", true, true);
        await service.SetAsync(next);
        Assert.Equal(next, service.State);
        Assert.True(service.Language.Arabic);
        Assert.Equal(2, changed);
        Assert.Equal(next, js.LastApplied);
        Assert.True(js.Save);
    }

    [Fact]
    public async Task Browser_theme_sync_keeps_the_latest_choice_without_reapplying_it()
    {
        var js = new ThemeJsRuntime();
        var service = new ThemeService(js);
        await service.InitializeAsync();
        var language = service.Language;
        js.BrowserTheme = new ThemeState("flat", "amber", true, false);
        await service.RefreshFromBrowserAsync();
        Assert.Equal(js.BrowserTheme, service.State);
        Assert.Same(language, service.Language);
        Assert.Null(js.LastApplied);
        js.BrowserTheme = js.BrowserTheme with { Rtl = true };
        await service.RefreshFromBrowserAsync();
        Assert.True(service.Language.Arabic);
    }

    [Fact]
    public void User_preferences_and_page_working_sets_are_not_shared_between_users()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IJSRuntime>(new ThemeJsRuntime());
        services.AddDashboardApplication(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var first = provider.CreateScope();
        using var second = provider.CreateScope();
        Assert.NotSame(first.ServiceProvider.GetRequiredService<ThemeService>(), second.ServiceProvider.GetRequiredService<ThemeService>());
        Assert.NotSame(first.ServiceProvider.GetRequiredService<CustomerService>(), first.ServiceProvider.GetRequiredService<CustomerService>());
    }

    [Theory]
    [InlineData("Customers")]
    [InlineData("Tasks")]
    [InlineData("Settings")]
    [InlineData("Task details")]
    [InlineData("Good morning, Noura")]
    [InlineData("Revenue trend")]
    public void Translation_catalogs_keep_existing_keys(string key)
    {
        Assert.Equal(key, new Ui(false)[key]);
        Assert.NotEqual(key, new Ui(true)[key]);
        Assert.Equal("Future text", new Ui(true)["Future text"]);
        Assert.Equal("1234", 1234.ToString("0", new Ui(true).Culture));
    }
}

internal sealed class ThemeJsRuntime : IJSRuntime
{
    public ThemeState BrowserTheme { get; set; } = new("glass", "indigo", false, false);
    public int LoadCount { get; private set; }
    public ThemeState? LastApplied { get; private set; }
    public bool Save { get; private set; }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => InvokeAsync<TValue>(identifier, default, args);
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        if (identifier == "dashTheme.load")
        {
            LoadCount++;
            return ValueTask.FromResult((TValue)(object)BrowserTheme);
        }
        if (identifier == "dashTheme.apply")
        {
            LastApplied = (ThemeState)args![0]!;
            Save = (bool)args[1]!;
        }
        return ValueTask.FromResult(default(TValue)!);
    }
}
