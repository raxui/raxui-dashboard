using System.ComponentModel.DataAnnotations;
using raxui_dashboard.Features.Tasks.Models;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Features.Tasks.Services;

/// <summary>Task operations are independent of Razor and the Kanban component's display types.</summary>
public sealed class TaskService
{
    private readonly DashboardData data;
    private int nextId = 11;
    public IReadOnlyList<TaskColumn> Columns { get; private set; }

    public TaskService(DashboardData data)
    {
        this.data = data;
        Columns = data.GetTaskBoard().Select(c => new TaskColumn(c.Column, c.Cards, TaskOptions.Meta[c.Column].Limit)).ToArray();
    }

    public void SetColumns(IReadOnlyList<TaskColumn> columns) => Columns = columns;
    public string ColumnOf(string id) => Columns.FirstOrDefault(c => c.Items.Any(i => i.Id == id))?.Id ?? "backlog";
    public bool IsDone(TaskCard card) => Columns.First(c => c.Id == "done").Items.Contains(card);
    public IEnumerable<string> TeamOptions => TaskOptions.Team.Union(Columns.SelectMany(c => c.Items).Select(i => i.Assignee)).Order();

    public async Task<TaskCard> SaveAsync(TaskDraft draft, string? editingId)
    {
        Validator.ValidateObject(draft, new ValidationContext(draft), validateAllProperties: true);
        await data.SaveAsync();
        var id = editingId ?? $"t{nextId++}";
        var old = Columns.SelectMany(c => c.Items).FirstOrDefault(i => i.Id == id);
        var card = new TaskCard(id, draft.Title.Trim(), draft.Tag, draft.Priority, draft.Assignee, draft.Due, old?.Comments ?? 0);
        var from = old is null ? null : ColumnOf(id);
        Columns = Columns.Select(c =>
            c.Id == from && from == draft.Column ? c with { Items = c.Items.Select(i => i.Id == id ? card : i).ToArray() }
            : c.Id == from ? c with { Items = c.Items.Where(i => i.Id != id).ToArray() }
            : c.Id == draft.Column ? c with { Items = [card, .. c.Items] }
            : c).ToArray();
        return card;
    }

    public async Task DeleteAsync(string id)
    {
        await data.SaveAsync();
        Columns = Columns.Select(c => c with { Items = c.Items.Where(i => i.Id != id).ToArray() }).ToArray();
    }
}
