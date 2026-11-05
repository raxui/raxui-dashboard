namespace raxui_dashboard.Services.Localization;

public sealed partial record Ui
{
    private static Dictionary<string, string> AssistantStrings() => new()
    {
        ["Ask about revenue, customers or tasks. Answers use the dashboard's own data."] = "اسأل عن الإيرادات أو العملاء أو المهام. الإجابات مبنية على بيانات اللوحة.",
        ["Demo model"] = "نموذج تجريبي",
        ["How can I help with your store today?"] = "كيف أساعدك في متجرك اليوم؟",
        ["How did revenue do this quarter?"] = "كيف كانت الإيرادات هذا الربع؟",
        ["Who are my top customers?"] = "من هم أفضل عملائي؟",
        ["What is overdue on the task board?"] = "ما المهام المتأخرة في لوحة المهام؟",
        ["Thinking…"] = "يفكّر…",
        ["Thought for {s}s"] = "فكّر لمدة {s} ث",
        ["Querying dashboard data"] = "الاستعلام عن بيانات اللوحة",
        ["Stopped"] = "أُوقف",
        ["Completed"] = "تم",
        ["Stopped."] = "أُوقف التوليد.",
        ["Waiting"] = "بانتظار",
        ["Running"] = "قيد التنفيذ",
        ["Failed"] = "فشل",
        ["Input"] = "المدخلات",
        ["Output"] = "النتيجة",
        ["Copy"] = "نسخ",
        ["Copied"] = "تم النسخ",
        ["Regenerate"] = "إعادة التوليد",
        ["Good response"] = "إجابة جيدة",
        ["Bad response"] = "إجابة سيئة",
        ["Ask about your store…"] = "اسأل عن متجرك…",
        ["Message"] = "الرسالة",
        ["Send"] = "إرسال",
        ["Stop"] = "إيقاف",
    };
}
