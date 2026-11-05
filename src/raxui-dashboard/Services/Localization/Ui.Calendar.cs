namespace raxui_dashboard.Services.Localization;

public sealed partial record Ui
{
    private static Dictionary<string, string> CalendarStrings() => new()
    {
        ["Meetings, reviews and launches for the team."] = "اجتماعات الفريق ومراجعاته وإطلاقاته.",
        ["Meeting"] = "اجتماع",
        ["Review"] = "مراجعة",
        ["Call"] = "مكالمة",
        ["Event"] = "فعالية",
        ["Launch"] = "إطلاق",
        ["event"] = "حدث",
        ["events"] = "أحداث",
        ["min"] = "دقيقة",
        ["completed"] = "مكتمل",
        ["in progress"] = "جارٍ الآن",
        ["upcoming"] = "قادم",
        ["Nothing scheduled. Enjoy the quiet day."] = "لا شيء مجدول. استمتع بيوم هادئ.",
        ["Upcoming"] = "القادم",
        ["All day"] = "طوال اليوم",
        ["The next events on the team calendar."] = "الأحداث القادمة في تقويم الفريق.",
        ["Weekly sales sync"] = "اجتماع المبيعات الأسبوعي",
        ["October revenue review"] = "مراجعة إيرادات أكتوبر",
        ["Supplier call: Gulf Logistics"] = "مكالمة المورّد: الخليج للخدمات اللوجستية",
        ["Product photo shoot"] = "تصوير المنتجات",
        ["Design review: onboarding"] = "مراجعة التصميم: تجربة البدء",
        ["1:1 with Yusuf"] = "لقاء فردي مع يوسف",
        ["Orders API go-live"] = "إطلاق واجهة الطلبات",
        ["Mobile onboarding handoff"] = "تسليم تصميم تجربة البدء للجوال",
        ["Refund policy published"] = "نشر سياسة الاسترداد",
        ["Black Friday launch"] = "إطلاق الجمعة البيضاء",
        ["Q1 forecast review"] = "مراجعة توقعات الربع الأول",
        ["Cyber Monday"] = "الاثنين الإلكتروني",
    };
}
