namespace raxui_dashboard.Services.Localization;

public sealed partial record Ui
{
    private static Dictionary<string, string> TasksStrings() => new()
    {
        ["Drag cards between columns with the mouse, touch or keyboard."] = "اسحب البطاقات بين الأعمدة بالفأرة أو اللمس أو لوحة المفاتيح.",
        ["New task"] = "مهمة جديدة",
        ["Backlog"] = "قائمة الانتظار",
        ["To do"] = "للتنفيذ",
        ["In progress"] = "قيد التنفيذ",
        ["Done"] = "منجزة",
        ["Drop cards here"] = "أفلت البطاقات هنا",
        ["Drag card"] = "اسحب البطاقة",
        ["Drag column"] = "اسحب العمود",
        ["To pick up a card or column, press Space or Enter. Use the arrow keys to move it, Space or Enter to drop it, and Escape to cancel."] =
            "لالتقاط بطاقة أو عمود اضغط مسافة أو Enter، ثم حرّكه بالأسهم، واضغط مسافة أو Enter للإفلات أو Escape للإلغاء.",
        ["Picked up {item} in {column}. Position {position} of {total}."] = "التقطت {item} في {column}. الموضع {position} من {total}.",
        ["{item} moved to {column}, position {position} of {total}."] = "نُقلت {item} إلى {column}، الموضع {position} من {total}.",
        ["{item} dropped in {column} at position {position} of {total}."] = "أُفلتت {item} في {column} في الموضع {position} من {total}.",
        ["Cancelled. {item} is back where it was."] = "أُلغي النقل. {item} عادت إلى مكانها.",
        ["Analytics"] = "التحليلات",
        ["Content"] = "المحتوى",
        ["Finance"] = "المالية",
        ["Marketing"] = "التسويق",
        ["Operations"] = "العمليات",
        ["Support"] = "الدعم",
        ["Design"] = "التصميم",
        ["Engineering"] = "الهندسة",
        ["Low"] = "منخفضة",
        ["Medium"] = "متوسطة",
        ["High"] = "عالية",
    };
}
