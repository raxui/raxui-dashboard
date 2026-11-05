namespace raxui_dashboard.Features.Assistant.Models;

public sealed class AssistantTurn
{
    public int Id; public string Prompt = ""; public AssistantAnswer Answer = default!;
    public string Reasoning = ""; public bool Thinking; public string? Tool; public string? Output;
    public string Text = ""; public bool Streaming, Done, Stopped; public string? Feedback;
}
