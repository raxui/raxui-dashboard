namespace raxui_dashboard.Features.Settings.Models;

public sealed class NotificationOption(string key, string hint, bool on)
{
    public string Key => key;
    public string Hint => hint;
    public bool On { get; set; } = on;
}
