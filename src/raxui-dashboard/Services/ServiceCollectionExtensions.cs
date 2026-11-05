using raxui_dashboard.Features.Analytics.Services;
using raxui_dashboard.Features.Assistant.Services;
using raxui_dashboard.Features.Calendar.Services;
using raxui_dashboard.Features.Crm.Services;
using raxui_dashboard.Features.Store.Services;
using raxui_dashboard.Features.Profile.Services;
using raxui_dashboard.Features.Settings.Services;
using raxui_dashboard.Features.Customers.Services;
using raxui_dashboard.Features.Tasks.Services;
using raxui_dashboard.Infrastructure.DemoData;
using raxui_dashboard.Services.Theme;
using raxui_dashboard.Services.Time;

namespace raxui_dashboard.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // The singleton contains only deterministic, read-only demo source data.
        services.AddSingleton<DashboardData>();
        // Mutable working sets are owned by the page that requests them, as before the refactor.
        services.AddTransient<CustomerService>();
        services.AddTransient<TaskService>();
        services.AddTransient<AnalyticsService>();
        services.AddTransient<AssistantAnswerService>();
        services.AddTransient<AssistantSession>();
        services.AddTransient<CalendarService>();
        services.AddTransient<CrmService>();
        services.AddTransient<StoreService>();
        services.AddTransient<ProfileService>();
        services.AddTransient<SettingsService>();
        services.AddScoped<ThemeService>();
        services.AddScoped<raxui_dashboard.Components.UI.Toast.Toaster.ToastService>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(sp => new AppClock(sp.GetRequiredService<TimeProvider>(),
            configuration["Application:TimeZone"] ?? "Asia/Baghdad"));
        return services;
    }
}
