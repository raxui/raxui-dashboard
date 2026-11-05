using System.Globalization;

namespace raxui_dashboard.Components.Layout;

/// <summary>UI language, cascaded from MainLayout. <c>L["Overview"]</c> returns the Arabic text when Arabic is on.</summary>
public sealed record Ui(bool Arabic)
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo Ar = CultureInfo.GetCultureInfo("ar-EG");

    public CultureInfo Culture => Arabic ? Ar : En;

    public string this[string english] => Arabic && Strings.TryGetValue(english, out var arabic) ? arabic : english;

    public string Money(decimal value, int decimals = 0) => value.ToString("C" + decimals, En);

    private static readonly Dictionary<string, string> Strings = new()
    {
        ["Overview"] = "نظرة عامة",
        ["Customers"] = "العملاء",
        ["Tasks"] = "المهام",
        ["Calendar"] = "التقويم",
        ["AI Assistant"] = "المساعد الذكي",
        ["Settings"] = "الإعدادات",
        ["Workspace"] = "مساحة العمل",
        ["Navigation"] = "التنقل",
        ["Main navigation"] = "التنقل الرئيسي",
        ["Admin dashboard"] = "لوحة التحكم",
        ["Toggle sidebar"] = "تبديل الشريط الجانبي",
        ["Search…"] = "بحث…",
        ["Search pages and actions…"] = "ابحث في الصفحات والإجراءات…",
        ["Command palette"] = "لوحة الأوامر",
        ["No results found."] = "لا توجد نتائج.",
        ["Pages"] = "الصفحات",
        ["Theme"] = "المظهر",
        ["Appearance"] = "النمط",
        ["Accent"] = "اللون",
        ["Default"] = "افتراضي",
        ["Flat"] = "مسطّح",
        ["Glass"] = "زجاجي",
        ["Blue"] = "أزرق",
        ["Emerald"] = "زمردي",
        ["Indigo"] = "نيلي",
        ["Amber"] = "كهرماني",
        ["Dark mode"] = "الوضع الداكن",
        ["Light mode"] = "الوضع الفاتح",
        ["Switch to dark mode"] = "التبديل إلى الوضع الداكن",
        ["Switch to light mode"] = "التبديل إلى الوضع الفاتح",
        ["العربية"] = "English",
        ["Switch to Arabic"] = "Switch to English",
        ["Language"] = "اللغة",
        ["Account"] = "الحساب",
        ["Profile"] = "الملف الشخصي",
        ["Billing"] = "الفوترة",
        ["Sign out"] = "تسجيل الخروج",
        ["Account menu"] = "قائمة الحساب",
        ["Theme settings"] = "إعدادات المظهر",
        ["Pro plan"] = "خطة Pro",

        // Overview
        ["Good morning, Noura"] = "صباح الخير يا نورة",
        ["Here is how your store performed over the last 12 months."] = "هذا أداء متجرك خلال آخر 12 شهرًا.",
        ["Live"] = "مباشر",
        ["Revenue"] = "الإيرادات",
        ["Orders"] = "الطلبات",
        ["New customers"] = "عملاء جدد",
        ["Avg. order value"] = "متوسط قيمة الطلب",
        ["vs last month"] = "مقارنة بالشهر الماضي",
        ["Revenue trend"] = "اتجاه الإيرادات",
        ["Monthly revenue in thousands of dollars, compared with last year."] = "الإيرادات الشهرية بآلاف الدولارات مقارنة بالعام الماضي.",
        ["This year"] = "هذا العام",
        ["Last year"] = "العام الماضي",
        ["Orders by month"] = "الطلبات حسب الشهر",
        ["New and returning customers."] = "العملاء الجدد والعائدون.",
        ["New"] = "جدد",
        ["Returning"] = "عائدون",
        ["Sales by channel"] = "المبيعات حسب القناة",
        ["Share of orders this month."] = "حصة الطلبات هذا الشهر.",
        ["Online store"] = "المتجر الإلكتروني",
        ["Mobile app"] = "تطبيق الجوال",
        ["Marketplace"] = "السوق الإلكتروني",
        ["Retail"] = "المتاجر",
        ["Recent orders"] = "أحدث الطلبات",
        ["The latest orders across all channels."] = "آخر الطلبات من جميع القنوات.",
        ["View all"] = "عرض الكل",
        ["Order"] = "الطلب",
        ["Customer"] = "العميل",
        ["Channel"] = "القناة",
        ["Date"] = "التاريخ",
        ["Status"] = "الحالة",
        ["Amount"] = "المبلغ",
        ["Paid"] = "مدفوع",
        ["Shipped"] = "تم الشحن",
        ["Pending"] = "قيد الانتظار",
        ["Refunded"] = "مسترد",
        ["Cancelled"] = "ملغى",

        // Placeholder pages
        ["Coming soon"] = "قريبًا",
        ["This page is part of the next slice of the dashboard."] = "هذه الصفحة جزء من المرحلة القادمة من لوحة التحكم.",
        ["Back to overview"] = "العودة إلى النظرة العامة",
    };
}
