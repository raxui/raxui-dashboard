namespace raxui_dashboard.Services.Localization;

public sealed partial record Ui
{
    private static Dictionary<string, string> OverviewStrings() => new()
    {
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
    };
}
