namespace raxui_dashboard.Services.Localization;

public sealed partial record Ui
{
    private static Dictionary<string, string> CustomersStrings() => new()
    {
        ["Search, filter and manage everyone who bought from your store."] = "ابحث وصفِّ وأدر كل من اشترى من متجرك.",
        ["Add customer"] = "إضافة عميل",
        ["Summary"] = "ملخص",
        ["Active"] = "نشط",
        ["Trial"] = "تجريبي",
        ["Paused"] = "متوقف",
        ["Churned"] = "مغادر",
        ["Lifetime value"] = "القيمة الإجمالية",
        ["Countries represented"] = "الدول الممثّلة",
        ["Customer details"] = "بيانات العميل",
        ["Account details"] = "تفاصيل الحساب",
        ["Customer ID"] = "رقم العميل",
        ["Of all customers"] = "من إجمالي العملاء",
        ["Average per customer"] = "المتوسط لكل عميل",
        ["Total orders"] = "إجمالي الطلبات",
        ["Avg. orders"] = "متوسط الطلبات",
        ["Export"] = "تصدير",
        ["Export started"] = "بدأ التصدير",
        ["Delete"] = "حذف",
        ["Deleted"] = "تم الحذف",
        ["Country"] = "الدولة",
        ["Plan"] = "الخطة",
        ["Spent"] = "الإنفاق",
        ["Last order"] = "آخر طلب",
        ["Free"] = "مجانية",
        ["Pro"] = "Pro",
        ["Business"] = "أعمال",
        ["Enterprise"] = "مؤسسات",
        ["Iraq"] = "العراق",
        ["UAE"] = "الإمارات",
        ["Germany"] = "ألمانيا",
        ["United States"] = "الولايات المتحدة",
        ["Egypt"] = "مصر",
        ["France"] = "فرنسا",
        ["Jordan"] = "الأردن",
        ["United Kingdom"] = "المملكة المتحدة",
    };
}
