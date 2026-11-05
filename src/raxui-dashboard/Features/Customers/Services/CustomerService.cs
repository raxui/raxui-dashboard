using System.ComponentModel.DataAnnotations;
using raxui_dashboard.Features.Customers.Models;
using raxui_dashboard.Infrastructure.DemoData;
using raxui_dashboard.Services.Time;

namespace raxui_dashboard.Features.Customers.Services;

/// <summary>Page-owned working set. Demo changes are intentionally reset on a new page instance.</summary>
public sealed class CustomerService(DashboardData data, AppClock clock)
{
    public IReadOnlyList<Customer> Items { get; private set; } = [];
    private int nextId = 2042;

    public async Task LoadAsync() => Items = await data.GetCustomersAsync();

    public async Task SaveAsync(CustomerDraft draft, string? editingId)
    {
        Validator.ValidateObject(draft, new ValidationContext(draft), validateAllProperties: true);
        await data.SaveAsync();
        if (editingId is { } id)
        {
            Items = Items.Select(c => c.Id != id ? c : c with
            {
                Name = draft.Name.Trim(), Email = draft.Email.Trim(), Country = draft.Country,
                Plan = draft.Plan, Status = draft.Status,
            }).ToArray();
        }
        else
        {
            var customer = new Customer($"C-{nextId++}", draft.Name.Trim(), draft.Email.Trim(), draft.Country,
                draft.Plan, draft.Trial ? "Trial" : "Active", 0, 0, clock.Today);
            Items = [customer, .. Items];
        }
    }

    public async Task DeleteAsync(IEnumerable<string> ids)
    {
        var selected = ids.ToHashSet();
        await data.SaveAsync();
        Items = Items.Where(c => !selected.Contains(c.Id)).ToArray();
    }
}
