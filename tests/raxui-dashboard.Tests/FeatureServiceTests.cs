using System.ComponentModel.DataAnnotations;
using raxui_dashboard.Features.Customers.Models;
using raxui_dashboard.Features.Customers.Services;
using raxui_dashboard.Features.Tasks.Models;
using raxui_dashboard.Features.Tasks.Services;
using raxui_dashboard.Infrastructure.DemoData;
using raxui_dashboard.Services.Localization;
using raxui_dashboard.Services.Time;

namespace raxui_dashboard.Tests;

public class FeatureServiceTests
{
    private static readonly Ui English = new(false);
    private static readonly DateOnly Today = new(2026, 11, 5);
    private static AppClock Clock => new(new FixedTimeProvider(new(2026, 11, 5, 8, 0, 0, TimeSpan.Zero)));

    [Fact]
    public async Task Customer_edit_preserves_purchase_history_and_bulk_delete_removes_only_selected_ids()
    {
        var service = new CustomerService(new DashboardData(), Clock);
        await service.LoadAsync();
        Assert.Equal(36, service.Items.Count);
        var original = service.Items[0];
        await service.SaveAsync(new(English)
        {
            Name = " Updated Name ", Email = "updated@example.test", Country = "Iraq", Plan = "Pro", Status = "Paused",
        }, original.Id);
        var updated = service.Items[0];
        Assert.Equal(original.Id, updated.Id);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal(original.Orders, updated.Orders);
        Assert.Equal(original.Spent, updated.Spent);
        Assert.Equal(original.LastOrder, updated.LastOrder);
        var removed = service.Items.Take(2).Select(c => c.Id).ToArray();
        await service.DeleteAsync([removed[0], removed[1], removed[0]]);
        Assert.Equal(34, service.Items.Count);
        Assert.DoesNotContain(service.Items, c => removed.Contains(c.Id));
    }

    [Fact]
    public async Task New_customers_have_unique_ids_trial_status_and_no_purchase_history()
    {
        var service = new CustomerService(new DashboardData(), Clock);
        await service.LoadAsync();
        var draft = new CustomerDraft(English) { Name = "New Customer", Email = "new@example.test", Trial = true };
        await service.SaveAsync(draft, null);
        await service.SaveAsync(draft, null);
        Assert.Equal(38, service.Items.Count);
        Assert.Equal(38, service.Items.Select(c => c.Id).Distinct().Count());
        var customer = service.Items[0];
        Assert.Equal("Trial", customer.Status);
        Assert.Equal(0, customer.Orders);
        Assert.Equal(0, customer.Spent);
        Assert.Equal(Clock.Today, customer.LastOrder);
    }

    [Fact]
    public async Task Invalid_customer_cannot_modify_the_working_set()
    {
        var service = new CustomerService(new DashboardData(), Clock);
        await service.LoadAsync();
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(new(English) { Name = "", Email = "invalid" }, null));
        Assert.Equal(36, service.Items.Count);
    }

    [Fact]
    public async Task Task_edit_preserves_order_and_comments_then_moves_without_duplication()
    {
        var service = new TaskService(new DashboardData());
        var before = service.Columns.First(c => c.Id == "todo").Items;
        var original = before[1];
        var draft = new TaskDraft(English, Today)
        {
            Title = " Updated task ", Column = "todo", Priority = "High", Tag = original.Tag,
            Assignee = original.Assignee, Due = null,
        };
        var updated = await service.SaveAsync(draft, original.Id);
        Assert.Equal(before.Select(c => c.Id), service.Columns.First(c => c.Id == "todo").Items.Select(c => c.Id));
        Assert.Equal(original.Comments, updated.Comments);
        Assert.Equal("Updated task", updated.Title);
        Assert.Null(updated.Due);
        draft.Column = "done";
        var moved = await service.SaveAsync(draft, original.Id);
        Assert.True(service.IsDone(moved));
        Assert.Equal("done", service.ColumnOf(original.Id));
        Assert.Equal(10, service.Columns.Sum(c => c.Items.Count));
        Assert.Single(service.Columns.SelectMany(c => c.Items), c => c.Id == original.Id);
        await service.DeleteAsync(original.Id);
        Assert.Equal(9, service.Columns.Sum(c => c.Items.Count));
    }

    [Fact]
    public async Task New_task_defaults_and_validation_remain_consistent()
    {
        var service = new TaskService(new DashboardData());
        var draft = new TaskDraft(English, Today);
        Assert.Equal("todo", draft.Column);
        Assert.Equal("Medium", draft.Priority);
        Assert.Equal(Today.AddDays(7), draft.Due);
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(draft, null));
        draft.Title = new string('x', 81);
        await Assert.ThrowsAsync<ValidationException>(() => service.SaveAsync(draft, null));
        draft.Title = "Valid task";
        var created = await service.SaveAsync(draft, null);
        Assert.Equal("t11", created.Id);
        Assert.Equal(11, service.Columns.Sum(c => c.Items.Count));
    }

    [Fact]
    public void Board_reordering_keeps_ids_limits_and_current_status()
    {
        var service = new TaskService(new DashboardData());
        var doing = service.Columns.First(c => c.Id == "doing");
        var card = doing.Items[0];
        service.SetColumns(service.Columns.Reverse().Select(c => c.Id == "doing"
            ? c with { Items = c.Items.Where(i => i.Id != card.Id).ToArray() }
            : c.Id == "todo" ? c with { Items = [card, .. c.Items] } : c).ToArray());
        Assert.Equal("done", service.Columns[0].Id);
        Assert.Equal("todo", service.ColumnOf(card.Id));
        Assert.Equal(3, service.Columns.First(c => c.Id == "doing").Limit);
        Assert.Equal(10, service.Columns.Sum(c => c.Items.Count));
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset value) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => value;
}
