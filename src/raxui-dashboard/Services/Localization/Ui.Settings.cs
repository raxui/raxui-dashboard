namespace raxui_dashboard.Services.Localization;

public sealed partial record Ui
{
    private static Dictionary<string, string> SettingsStrings() => new()
    {
        ["Manage your appearance, language and notifications."] = "أدر المظهر واللغة والإشعارات.",
        ["Personal information"] = "المعلومات الشخصية",
        ["Manage the details your team sees on your profile."] = "أدر المعلومات التي يراها فريقك في ملفك الشخصي.",
        ["Make it yours"] = "مساحة عمل بطابعك",
        ["Choose how your workspace looks and feels."] = "اختر مظهر مساحة العمل بما يناسبك.",
        ["Changes apply instantly"] = "تُطبّق التغييرات فورًا",
        ["Stay in the loop"] = "ابقَ على اطلاع",
        ["Choose which updates arrive in your inbox."] = "اختر التحديثات التي تصلك عبر البريد الإلكتروني.",
        ["Manage your profile, appearance and notifications."] = "أدر ملفك الشخصي والمظهر والإشعارات.",
        ["Full name"] = "الاسم الكامل",
        ["Email"] = "البريد الإلكتروني",
        ["Role"] = "الدور",
        ["Admin"] = "مدير النظام",
        ["Manager"] = "مدير",
        ["Analyst"] = "محلل",
        ["Bio"] = "نبذة",
        ["Shown to your team."] = "تظهر لفريقك.",
        ["(Optional)"] = "(اختياري)",
        ["Save changes"] = "حفظ التغييرات",
        ["Saved"] = "تم الحفظ",
        ["Default, flat or frosted glass surfaces."] = "أسطح افتراضية أو مسطحة أو زجاجية.",
        ["Arabic interface"] = "الواجهة العربية",
        ["Right-to-left layout with Arabic text."] = "تخطيط من اليمين إلى اليسار بنصوص عربية.",
        ["Saved in this browser and applied before the page paints."] = "يُحفظ في هذا المتصفح ويُطبَّق قبل ظهور الصفحة.",
        ["Notifications"] = "الإشعارات",
        ["New orders"] = "الطلبات الجديدة",
        ["Weekly report"] = "التقرير الأسبوعي",
        ["Low stock"] = "انخفاض المخزون",
        ["Product news"] = "أخبار المنتج",
        ["An email for every order over $250."] = "رسالة لكل طلب يتجاوز 250$.",
        ["Revenue and orders every Monday."] = "الإيرادات والطلبات كل يوم اثنين.",
        ["When a product falls below 10 units."] = "عندما يقل منتج عن 10 وحدات.",
        ["Occasional updates about new features."] = "تحديثات متفرقة عن الميزات الجديدة.",
        ["Renews on 1 Dec 2026"] = "يتجدد في 1 ديسمبر 2026",
    };
}
