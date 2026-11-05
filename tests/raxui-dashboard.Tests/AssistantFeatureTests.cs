using System.Text.Json;
using raxui_dashboard.Features.Assistant.Services;
using raxui_dashboard.Infrastructure.DemoData;

namespace raxui_dashboard.Tests;

public class AssistantFeatureTests
{
    [Theory]
    [InlineData("How did revenue do?", false, "query_revenue")]
    [InlineData("Who are my top customers?", false, "query_customers")]
    [InlineData("What is overdue on the task board?", false, "query_tasks")]
    [InlineData("Who are my top customers?", true, "query_customers")]
    public async Task Demo_answer_routing_preserves_data_tools(string prompt, bool arabic, string tool)
    {
        var answer = await new AssistantAnswerService(new DashboardData()).AnswerAsync(prompt, arabic);
        Assert.Equal(tool, answer.Tool);
        Assert.NotEmpty(answer.Text);
        Assert.NotEmpty(answer.Reasoning);
        using var input = JsonDocument.Parse(answer.Input);
        using var output = JsonDocument.Parse(answer.Output);
        Assert.Equal(JsonValueKind.Object, input.RootElement.ValueKind);
        if (tool == "query_customers") Assert.Equal(5, output.RootElement.GetProperty("results").GetInt32());
        if (tool == "query_tasks") Assert.Equal(3, output.RootElement.GetProperty("due_soon").GetInt32());
        if (arabic) Assert.Contains(answer.Text, c => c is >= '\u0600' and <= '\u06ff');
    }

    [Fact]
    public async Task Arabic_prompt_is_detected_even_when_the_interface_is_English()
    {
        var answer = await new AssistantAnswerService(new DashboardData()).AnswerAsync("من أفضل العملاء؟", false);
        Assert.Equal("query_customers", answer.Tool);
        Assert.Contains("العملاء", answer.Reasoning);
    }

    [Fact]
    public async Task Stopping_a_stream_finishes_the_turn_and_restores_ready_state()
    {
        using var session = new AssistantSession(new(new DashboardData()), TimeProvider.System);
        var notifications = 0;
        session.Changed += () => notifications++;
        var sending = session.SendAsync("top customers", false);
        Assert.Equal("submitted", session.Status);
        Assert.Single(session.Turns);
        session.Stop();
        await sending;
        Assert.Equal("ready", session.Status);
        Assert.True(session.Turns[0].Done);
        Assert.True(session.Turns[0].Stopped);
        Assert.False(session.Turns[0].Streaming);
        Assert.True(notifications >= 2);
    }

    [Fact]
    public async Task Disposing_a_running_session_cancels_without_late_notifications()
    {
        var session = new AssistantSession(new(new DashboardData()), TimeProvider.System);
        var notifications = 0;
        session.Changed += () => notifications++;
        var sending = session.SendAsync("revenue", false);
        session.Dispose();
        var atDispose = notifications;
        await sending;
        Assert.Equal(atDispose, notifications);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => session.SendAsync("revenue", false));
    }
}
