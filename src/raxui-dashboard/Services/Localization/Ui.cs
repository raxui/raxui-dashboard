using System.Globalization;

namespace raxui_dashboard.Services.Localization;

/// <summary>UI language, cascaded from MainLayout. <c>L["Overview"]</c> returns the Arabic text when Arabic is on.</summary>
public sealed partial record Ui(bool Arabic)
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");
    // Arabic month and day names with Latin digits and separators, so numbers read the same in both languages.
    private static readonly CultureInfo Ar = CreateArabic();

    private static CultureInfo CreateArabic()
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("ar-EG").Clone();
        culture.NumberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        return CultureInfo.ReadOnly(culture);
    }

    public CultureInfo Culture => Arabic ? Ar : En;

    public string this[string english] => Arabic && Strings.TryGetValue(english, out var arabic) ? arabic : english;

    public string Money(decimal value, int decimals = 0) => value.ToString("C" + decimals, En);

    private static readonly IReadOnlyDictionary<string, string> Strings = CreateStrings();

    private static IReadOnlyDictionary<string, string> CreateStrings()
    {
        // Preserve the original indexer-initializer semantics: later sections may override a shared key.
        var result = new Dictionary<string, string>();
        foreach (var item in CommonStrings()) result[item.Key] = item.Value;
        foreach (var item in OverviewStrings()) result[item.Key] = item.Value;
        foreach (var item in CustomersStrings()) result[item.Key] = item.Value;
        foreach (var item in TasksStrings()) result[item.Key] = item.Value;
        foreach (var item in CalendarStrings()) result[item.Key] = item.Value;
        foreach (var item in AssistantStrings()) result[item.Key] = item.Value;
        foreach (var item in SettingsStrings()) result[item.Key] = item.Value;
        foreach (var item in StoreStrings()) result[item.Key] = item.Value;
        foreach (var item in CrmStrings()) result[item.Key] = item.Value;
        foreach (var item in AnalyticsStrings()) result[item.Key] = item.Value;
        foreach (var item in DialogsStrings()) result[item.Key] = item.Value;
        foreach (var item in NotificationsStrings()) result[item.Key] = item.Value;
        return result;
    }
}
