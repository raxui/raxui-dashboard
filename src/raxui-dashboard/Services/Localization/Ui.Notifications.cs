namespace raxui_dashboard.Services.Localization;

public sealed partial record Ui
{
    private static Dictionary<string, string> NotificationsStrings() => new()
    {
        ["Dismiss notification"] = "إغلاق الإشعار",
        ["Profile saved"] = "تم حفظ الملف الشخصي",
        ["Preferences saved"] = "تم حفظ التفضيلات",
        ["Task added"] = "تمت إضافة المهمة",
        ["Task updated"] = "تم تحديث المهمة",
        ["Board updated"] = "تم تحديث اللوحة",
        ["Could not save customer"] = "تعذّر حفظ العميل",
        ["Could not delete customers"] = "تعذّر حذف العملاء",
        ["Could not save task"] = "تعذّر حفظ المهمة",
        ["Could not delete task"] = "تعذّر حذف المهمة",
        ["Could not save profile"] = "تعذّر حفظ الملف الشخصي",
        ["Could not save preferences"] = "تعذّر حفظ التفضيلات",
        ["Please try again."] = "يرجى المحاولة مرة أخرى.",
        ["{0} customers"] = "{0} عملاء",
        ["Demo export"] = "تصدير تجريبي",
        ["{0} customers selected. Export is simulated; no file was downloaded."] = "تم تحديد {0} عملاء. التصدير تجريبي ولم يتم تنزيل ملف.",
        ["Notification preference updated"] = "تم تحديث تفضيل الإشعارات",
        ["Demo setting; resets when you leave this page."] = "إعداد تجريبي يُعاد ضبطه عند مغادرة الصفحة.",
    };
}
