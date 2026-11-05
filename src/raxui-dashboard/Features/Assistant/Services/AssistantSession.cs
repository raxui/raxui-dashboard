using raxui_dashboard.Features.Assistant.Models;

namespace raxui_dashboard.Features.Assistant.Services;

public sealed class AssistantSession(AssistantAnswerService answers, TimeProvider timeProvider) : IDisposable
{
    private readonly List<AssistantTurn> turns = [];
    private string status = "ready";
    private int nextId = 1;
    private CancellationTokenSource? run;
    private bool disposed;
    public IReadOnlyList<AssistantTurn> Turns => turns;
    public string Status => status;
    public event Action? Changed;
    private void Notify() => Changed?.Invoke();

    // Simulated model: think line by line, call a tool, then stream the answer word by word.
    public async Task SendAsync(string prompt, bool arabic)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        run?.Cancel();
        run?.Dispose();
        run = new();
        var token = run.Token;
        var turn = new AssistantTurn { Id = nextId++, Prompt = prompt, Answer = await answers.AnswerAsync(prompt, arabic) };
        turns.Add(turn);
        status = "submitted";
        Notify();
        async Task Step(int ms, Action change) { await Task.Delay(TimeSpan.FromMilliseconds(ms), timeProvider, token); change(); Notify(); }
        try
        {
            await Step(400, () => { status = "streaming"; turn.Thinking = true; });
            var lines = turn.Answer.Reasoning.Split('\n');
            for (var i = 0; i < lines.Length; i++) { var line = lines[i]; var first = i == 0; await Step(500, () => turn.Reasoning += (first ? "" : "\n") + line); }
            await Step(400, () => { turn.Thinking = false; turn.Tool = "running"; });
            await Step(900, () => { turn.Tool = "completed"; turn.Output = turn.Answer.Output; });
            await Step(250, () => turn.Streaming = true);
            var words = turn.Answer.Text.Split(' ');
            for (var i = 0; i < words.Length; i++) { var word = words[i]; var first = i == 0; await Step(35, () => turn.Text += (first ? "" : " ") + word); }
            await Step(150, () => Finish(turn, false));
        }
        catch (TaskCanceledException) { }
    }

    private void Finish(AssistantTurn turn, bool stopped)
    {
        turn.Thinking = false; turn.Streaming = false; turn.Done = true; turn.Stopped = stopped;
        if (turn.Tool == "running") turn.Tool = "error";
        status = "ready";
    }

    public void Stop() { run?.Cancel(); if (turns.Count > 0) { Finish(turns[^1], true); Notify(); } }


    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        run?.Cancel();
        run?.Dispose();
        Changed = null;
    }
}
