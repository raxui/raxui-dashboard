namespace raxui_dashboard.Features.Tasks.Models;

public sealed record TaskCard(string Id, string Title, string Tag, string Priority, string Assignee, DateOnly? Due, int Comments);
