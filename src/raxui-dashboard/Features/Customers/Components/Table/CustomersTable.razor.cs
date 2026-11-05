using Microsoft.AspNetCore.Components;
using raxui_dashboard.Components.UI.DataTable;
using raxui_dashboard.Components.UI.Toast;
using raxui_dashboard.Features.Customers.Models;
using raxui_dashboard.Services.Localization;

namespace raxui_dashboard.Features.Customers.Components.Table;

public partial class CustomersTable
{
    [CascadingParameter] public Ui L { get; set; } = new(false);
    [Parameter, EditorRequired] public IReadOnlyList<Customer> Customers { get; set; } = [];
    [Inject] public Toaster.ToastService Toasts { get; set; } = default!;
    [Parameter] public EventCallback<Customer> OnView { get; set; }
    [Parameter] public EventCallback<Customer> OnEdit { get; set; }
    [Parameter] public EventCallback<IReadOnlyList<Customer>> OnDelete { get; set; }

    private DataTable<Customer>? table;

    public Task ClearSelection() => table?.ClearSelection() ?? Task.CompletedTask;

    private Task Export(int count)
    {
        Toasts.Show(L["Demo export"], description: string.Format(L["{0} customers selected. Export is simulated; no file was downloaded."], count));
        return Task.CompletedTask;
    }

    private async Task ExportSelected(IReadOnlyList<Customer> rows)
    {
        await Export(rows.Count);
        await ClearSelection();
    }

    private DataTableTypes.Option[] Options(params string[] values) => values.Select(v => new DataTableTypes.Option(v, L[v])).ToArray();

    private IReadOnlyList<DataTableTypes.Column<Customer>> Columns =>
    [
        new() { Id = "id", Title = "ID", Value = c => c.Id },
        new() { Id = "customer", Title = L["Customer"], Value = c => c.Name + " " + c.Email, Cell = CustomerCell, Hideable = false },
        new() { Id = "country", Title = L["Country"], Value = c => c.Country, Format = v => L[(string)v!],
                Options = Options(Customers.Select(c => c.Country).Distinct().Order().ToArray()) },
        new() { Id = "plan", Title = L["Plan"], Value = c => c.Plan, Cell = PlanCell, Options = Options("Free", "Pro", "Business", "Enterprise") },
        new() { Id = "status", Title = L["Status"], Value = c => c.Status, Cell = StatusCell, Options = Options("Active", "Trial", "Paused", "Churned") },
        new() { Id = "orders", Title = L["Orders"], Value = c => c.Orders, Align = "end", Searchable = false },
        new() { Id = "spent", Title = L["Spent"], Value = c => c.Spent, Format = v => L.Money((decimal)v!, 2), Align = "end", Searchable = false },
        new() { Id = "last", Title = L["Last order"], Value = c => c.LastOrder, Format = v => ((DateOnly)v!).ToString("d MMM yyyy", L.Culture), Searchable = false },
        new() { Id = "actions", Title = L["Actions"], Value = c => c.Id, Cell = ActionsCell, Sortable = false, Searchable = false, Hideable = false, Align = "end" },
    ];

    private DataTableTypes.Labels Labels => !L.Arabic ? new() { Search = "Search customers…" } : new()
    {
        Search = "ابحث عن عميل…", View = "العرض", Columns = "الأعمدة", Compact = "مضغوط", Comfortable = "مريح",
        Reset = "مسح الفلاتر", Empty = "لا توجد نتائج.", SelectAll = "تحديد صفوف الصفحة", SelectRow = "تحديد العميل",
        ClearSelection = "إلغاء التحديد", RowsPerPage = "صفوف الصفحة", Previous = "السابق", Next = "التالي",
        Pagination = "الصفحات", MorePages = "صفحات أخرى", BulkActions = "إجراءات جماعية",
        Results = n => $"{n} نتيجة", Selected = n => $"{n} محدد", Page = n => $"صفحة {n}",
    };
}
