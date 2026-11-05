using raxui_dashboard.Features.Settings.Models;

namespace raxui_dashboard.Features.Settings.Services;

public sealed class SettingsService
{
    public IReadOnlyList<NotificationOption> Notifications { get; } =
    [
        new("New orders", "An email for every order over $250.", true),
        new("Weekly report", "Revenue and orders every Monday.", true),
        new("Low stock", "When a product falls below 10 units.", false),
        new("Product news", "Occasional updates about new features.", false),
    ];
}
