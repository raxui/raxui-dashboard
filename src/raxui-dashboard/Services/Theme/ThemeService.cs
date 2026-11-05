using Microsoft.JSInterop;
using raxui_dashboard.Services.Localization;

namespace raxui_dashboard.Services.Theme;

/// <summary>Browser preferences belong to one Blazor circuit, never to a singleton.</summary>
public sealed class ThemeService(IJSRuntime js)
{
    public ThemeState State { get; private set; } = new("glass", "indigo", false, false);
    public Ui Language { get; private set; } = new(false);
    public event Action? Changed;
    private bool initialized;

    // Called after the first interactive render, when browser storage is available.
    public async Task InitializeAsync()
    {
        if (initialized) return;
        var saved = await js.InvokeAsync<ThemeState>("dashTheme.load");
        initialized = true;
        Update(saved);
    }

    public async Task SetAsync(ThemeState next)
    {
        Update(next);
        await js.InvokeVoidAsync("dashTheme.apply", next, true);
    }

    // Browser theme controls already applied and saved their change locally.
    // Read the latest state rather than replaying an older click over a newer one.
    public async Task RefreshFromBrowserAsync() => Update(await js.InvokeAsync<ThemeState>("dashTheme.load"));

    private void Update(ThemeState next)
    {
        State = next;
        if (Language.Arabic != next.Rtl) Language = new(next.Rtl);
        Changed?.Invoke();
    }
}
