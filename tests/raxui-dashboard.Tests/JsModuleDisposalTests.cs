using System.Reflection;
using Microsoft.JSInterop;
using raxui_dashboard.Components.UI.Chart;
using raxui_dashboard.Components.UI.DropdownMenu;
using raxui_dashboard.Components.UI.Select;

namespace raxui_dashboard.Tests;

public class JsModuleDisposalTests
{
    [Theory]
    [InlineData("select", false)]
    [InlineData("select", true)]
    [InlineData("dropdown", false)]
    [InlineData("dropdown", true)]
    [InlineData("chart", false)]
    [InlineData("chart", true)]
    public async Task Canceled_browser_cleanup_does_not_escape_component_disposal(string kind, bool duringModuleRelease)
    {
        var component = Create(kind, new CleanupModule(new TaskCanceledException(), duringModuleRelease));
        await component.DisposeAsync();
    }

    [Theory]
    [InlineData("select")]
    [InlineData("dropdown")]
    [InlineData("chart")]
    public async Task Actual_javascript_errors_are_not_hidden(string kind)
    {
        var component = Create(kind, new CleanupModule(new JSException("Cleanup bug"), false));
        await Assert.ThrowsAsync<JSException>(async () => await component.DisposeAsync());
    }

    private static IAsyncDisposable Create(string kind, IJSObjectReference module)
    {
        IAsyncDisposable component = kind switch
        {
            "select" => new Select<string>(),
            "dropdown" => new DropdownMenu(),
            "chart" => new Chart(),
            _ => throw new ArgumentException(nameof(kind)),
        };
        // Supply the browser module without requiring an interactive renderer.
        var field = component.GetType().GetField(kind == "chart" ? "module" : "_module", BindingFlags.Instance | BindingFlags.NonPublic)!;
        field.SetValue(component, module);
        return component;
    }

    private sealed class CleanupModule(Exception error, bool duringModuleRelease) : IJSObjectReference
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            duringModuleRelease ? ValueTask.FromResult(default(TValue)!) : ValueTask.FromException<TValue>(error);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);

        public ValueTask DisposeAsync() => ValueTask.FromException(error);
    }
}
