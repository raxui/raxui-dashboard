namespace raxui_dashboard.Features.Assistant.Models;

public sealed record AssistantAnswer(string Tool, string Input, string Output, string Reasoning, string Text);
