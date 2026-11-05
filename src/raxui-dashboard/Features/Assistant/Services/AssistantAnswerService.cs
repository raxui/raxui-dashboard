using System.Text.Json;
using raxui_dashboard.Features.Assistant.Models;
using raxui_dashboard.Features.Customers.Models;
using raxui_dashboard.Features.Store.Models;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Features.Assistant.Services;

public sealed class AssistantAnswerService(DashboardData data)
{
    // shortcut: a keyword router with canned, data-backed answers stands in for a model; swap in a real chat client later.
    public async Task<AssistantAnswer> AnswerAsync(string prompt, bool arabicInterface)
    {
        var p = prompt.ToLowerInvariant();
        var arabic = arabicInterface || prompt.Any(c => c is >= '؀' and <= 'ۿ');
        if (p.Contains("customer") || p.Contains("عميل") || p.Contains("عملا")) return Customers(await data.GetCustomersAsync(), arabic);
        if (p.Contains("task") || p.Contains("overdue") || p.Contains("مهام") || p.Contains("متأخر")) return Tasks(arabic);
        return Revenue(await data.GetOverviewAsync(), arabic);
    }

    private static string Json(object value) => JsonSerializer.Serialize(value, new JsonSerializerOptions
    {
        WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    });

    private static AssistantAnswer Revenue(Overview o, bool ar)
    {
        var q = o.Revenue.TakeLast(3).Sum();
        var prev = o.Revenue.SkipLast(3).TakeLast(3).Sum();
        var yoy = o.RevenueLastYear.TakeLast(3).Sum();
        var rows = string.Join("\n", Enumerable.Range(9, 3).Select(i =>
            $"| {o.Months[i].ToString("MMM yyyy", System.Globalization.CultureInfo.GetCultureInfo(ar ? "ar-EG" : "en-US"))} | ${o.Revenue[i]:0.0}k | ${o.RevenueLastYear[i]:0.0}k |"));
        var growth = (q - prev) / prev * 100;
        var vsYear = (q - yoy) / yoy * 100;
        var text = ar
            ? $"حققت الإيرادات **${q:0.0}k** في آخر ثلاثة أشهر، بزيادة **{growth:0.0}%** عن الربع السابق و**{vsYear:0.0}%** عن نفس الفترة من العام الماضي.\n\n| الشهر | هذا العام | العام الماضي |\n|---|--:|--:|\n{rows}\n\nأبرز الملاحظات:\n\n- أكتوبر هو أفضل شهر في السنة.\n- نمو العملاء الجدد يقود معظم الزيادة.\n- راقب متوسط قيمة الطلب: ارتفاعه أبطأ من الإيرادات."
            : $"Revenue reached **${q:0.0}k** over the last three months, up **{growth:0.0}%** on the previous quarter and **{vsYear:0.0}%** year over year.\n\n| Month | This year | Last year |\n|---|--:|--:|\n{rows}\n\nWhat stands out:\n\n- October was the best month of the year.\n- New customers drive most of the growth.\n- Watch average order value: it is rising slower than revenue.";
        return new("query_revenue", Json(new { range = "last_3_months", compare = new[] { "previous_period", "last_year" } }),
            Json(new { total_k = Math.Round(q, 1), previous_k = Math.Round(prev, 1), last_year_k = Math.Round(yoy, 1) }),
            ar ? "المستخدم يسأل عن إيرادات الربع.\nسأجمع آخر ثلاثة أشهر وأقارنها بالربع السابق والعام الماضي." : "The user asks about quarterly revenue.\nI will sum the last three months and compare with the previous quarter and last year.",
            text);
    }

    private static AssistantAnswer Customers(IReadOnlyList<Customer> all, bool ar)
    {
        var top = all.OrderByDescending(c => c.Spent).Take(5).ToArray();
        var rows = string.Join("\n", top.Select((c, i) => $"| {i + 1} | {c.Name} | {c.Plan} | {c.Orders} | ${c.Spent:N0} |"));
        var share = top.Sum(c => c.Spent) / all.Sum(c => c.Spent) * 100;
        var text = ar
            ? $"أعلى خمسة عملاء من حيث الإنفاق يمثلون **{share:0}%** من إجمالي قيمة العملاء:\n\n| # | العميل | الخطة | الطلبات | الإنفاق |\n|--:|---|---|--:|--:|\n{rows}\n\nاقتراح: أرسل لهم عرض ولاء قبل **الجمعة البيضاء**."
            : $"Your five biggest customers account for **{share:0}%** of lifetime value:\n\n| # | Customer | Plan | Orders | Spent |\n|--:|---|---|--:|--:|\n{rows}\n\nSuggestion: send them a loyalty offer before **Black Friday**.";
        return new("query_customers", Json(new { sort = "spent_desc", limit = 5 }),
            Json(new { results = top.Length, top = top[0].Name }),
            ar ? "المستخدم يريد أفضل العملاء.\nسأرتّب العملاء حسب الإنفاق وآخذ أول خمسة." : "The user wants the top customers.\nI will sort customers by amount spent and take five.",
            text);
    }

    private AssistantAnswer Tasks(bool ar)
    {
        var today = DemoSchedule.Today;
        var open = data.GetTaskBoard().Where(c => c.Column != "done").SelectMany(c => c.Cards.Select(card => (c.Column, card))).ToArray();
        var soon = open.Where(x => x.card.Due is { } d && d <= today.AddDays(7)).OrderBy(x => x.card.Due).ToArray();
        var list = string.Join("\n", soon.Select(x => $"- [ ] **{x.card.Title}** · {x.card.Assignee} · {x.card.Due:yyyy-MM-dd}"));
        var text = ar
            ? $"لا توجد مهام متأخرة اليوم، لكن **{soon.Length}** مهام تستحق خلال الأيام السبعة القادمة:\n\n{list}\n\nعمود **قيد التنفيذ** فيه {open.Count(x => x.Column == "doing")} من 3 مهام، فهناك مكان لمهمة واحدة أخرى."
            : $"Nothing is overdue today, but **{soon.Length}** tasks are due in the next seven days:\n\n{list}\n\n**In progress** holds {open.Count(x => x.Column == "doing")} of 3 tasks, so there is room for one more.";
        return new("query_tasks", Json(new { status = "open", due_within_days = 7 }),
            Json(new { open = open.Length, due_soon = soon.Length }),
            ar ? "المستخدم يسأل عن المهام المتأخرة.\nسأفحص المهام المفتوحة وتواريخ استحقاقها." : "The user asks what is overdue.\nI will check open tasks and their due dates.",
            text);
    }

}
