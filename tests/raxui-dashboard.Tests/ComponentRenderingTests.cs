using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using raxui_dashboard.Features.Customers.Components;
using raxui_dashboard.Features.Tasks.Components;
using raxui_dashboard.Features.Tasks.Models;
using raxui_dashboard.Infrastructure.DemoData;
using raxui_dashboard.Services.Localization;
using raxui_dashboard.Services;
using raxui_dashboard.Features.Store.Components;
using raxui_dashboard.Features.Crm.Components;
using raxui_dashboard.Features.Analytics.Components;

namespace raxui_dashboard.Tests;

public class ComponentRenderingTests
{
    private sealed class RenderNavigation : NavigationManager
    {
        public RenderNavigation() => Initialize("http://localhost/", "http://localhost/");
        protected override void NavigateToCore(string uri, bool forceLoad) { }
    }

    private static async Task<string> Render<T>(Dictionary<string, object?> parameters) where T : IComponent
    {
        var services = new ServiceCollection().AddLogging().AddSingleton<IJSRuntime>(new ThemeJsRuntime());
        services.AddDashboardApplication(new ConfigurationBuilder().Build());
        services.AddSingleton<NavigationManager>(new RenderNavigation());
        await using var provider = services.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await renderer.RenderComponentAsync<T>(ParameterView.FromDictionary(parameters));
            return output.ToHtmlString();
        });
    }

    [Theory]
    [InlineData("store", "Total sales")]
    [InlineData("crm", "Monthly proposal goal")]
    [InlineData("analytics", "Traffic sources")]
    public async Task Reports_include_data_in_the_initial_html_before_interactivity(string page, string expected)
    {
        var html = page switch
        {
            "store" => await Render<StoreDashboard>(new()),
            "crm" => await Render<CrmDashboard>(new()),
            "analytics" => await Render<AnalyticsDashboard>(new()),
            _ => throw new ArgumentException(nameof(page)),
        };
        Assert.Contains(expected, html);
        Assert.DoesNotContain("aria-busy=\"true\"", html);
        Assert.DoesNotContain("rax-skeleton", html);
    }

    [Theory]
    [InlineData(false, 3)]
    [InlineData(true, 4)]
    public async Task Task_form_keeps_radio_choices_and_priority_before_due_date(bool editing, int expectedColumns)
    {
        var html = await Render<TaskForm>(new()
        {
            [nameof(TaskForm.Draft)] = new TaskDraft(new Ui(false), new(2026, 11, 5)),
            [nameof(TaskForm.Editing)] = editing,
            [nameof(TaskForm.Today)] = new DateOnly(2026, 11, 5),
        });
        Assert.Equal(expectedColumns, System.Text.RegularExpressions.Regex.Matches(html, "name=\"task-column\"").Count);
        Assert.Equal(3, System.Text.RegularExpressions.Regex.Matches(html, "name=\"task-priority\"").Count);
        Assert.True(html.IndexOf("task-priority-label", StringComparison.Ordinal) < html.IndexOf("task-due-label", StringComparison.Ordinal));
        Assert.Contains("dash-task-dialog", html);
        Assert.Contains("id=\"add-task\"", html);
    }

    [Fact]
    public async Task Customer_summary_and_sheet_keep_the_real_purchase_figures()
    {
        var customers = await new DashboardData().GetCustomersAsync();
        var summary = await Render<CustomerSummary>(new() { [nameof(CustomerSummary.Customers)] = customers });
        Assert.Contains("$111,972", summary);
        Assert.Contains("61.1%", summary);
        Assert.Contains("882", summary);
        var sheet = await Render<CustomerDetailsSheet>(new() { [nameof(CustomerDetailsSheet.Value)] = customers[0] });
        Assert.Contains("Layla Nasser", sheet);
        Assert.Contains("layla.nasser@mail.com", sheet);
        Assert.Contains("$60.00", sheet);
        Assert.Contains("C-2041", sheet);
    }

    [Fact]
    public async Task Completed_tasks_are_not_overdue_and_tasks_without_dates_are_explicit()
    {
        var task = new TaskCard("t9", "Completed task", "Finance", "Medium", "Emma Schmidt", new(2026, 11, 3), 2);
        var parameters = new Dictionary<string, object?>
        {
            [nameof(TaskDetailsSheet.Value)] = task,
            [nameof(TaskDetailsSheet.Column)] = "done",
            [nameof(TaskDetailsSheet.Today)] = new DateOnly(2026, 11, 5),
        };
        var completed = await Render<TaskDetailsSheet>(parameters);
        Assert.DoesNotContain("Overdue", completed);
        parameters[nameof(TaskDetailsSheet.Column)] = "todo";
        Assert.Contains("Overdue", await Render<TaskDetailsSheet>(parameters));
        parameters[nameof(TaskDetailsSheet.Value)] = task with { Due = null };
        Assert.Contains("No due date", await Render<TaskDetailsSheet>(parameters));
    }
}
