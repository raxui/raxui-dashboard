namespace raxui_dashboard.Features.Tasks.Models;

public sealed record TaskColumn(string Id, IReadOnlyList<TaskCard> Items, int? Limit = null);
