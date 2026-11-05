using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using raxui_dashboard.Components.UI.Toast;
using raxui_dashboard.Services;

namespace raxui_dashboard.Tests;

public class ToastIntegrationTests
{
    [Fact]
    public void Notifications_are_shared_within_one_circuit_and_isolated_from_other_circuits()
    {
        var services = new ServiceCollection().AddDashboardApplication(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var first = provider.CreateScope();
        using var second = provider.CreateScope();
        var toast = first.ServiceProvider.GetRequiredService<Toaster.ToastService>();
        var id = toast.Show("Customer added", "success");
        Assert.Same(toast, first.ServiceProvider.GetRequiredService<Toaster.ToastService>());
        Assert.Empty(second.ServiceProvider.GetRequiredService<Toaster.ToastService>().Items);
        toast.Remove(id);
        Assert.Empty(toast.Items);
    }

    [Fact]
    public async Task Library_outlet_announces_success_and_errors_and_encodes_user_content()
    {
        var toast = new Toaster.ToastService();
        toast.Show("Customer added", "success", "<script>customer</script>");
        toast.Show("Could not save task", "danger");
        var services = new ServiceCollection().AddLogging().AddSingleton<IJSRuntime>(new ThemeJsRuntime()).AddSingleton(toast);
        await using var provider = services.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<Toaster>(ParameterView.Empty)).ToHtmlString());
        Assert.Contains("role=\"status\"", html);
        Assert.Contains("role=\"alert\"", html);
        Assert.Contains("aria-live=\"assertive\"", html);
        Assert.Contains("Customer added", html);
        Assert.Contains("&lt;script&gt;customer&lt;/script&gt;", html);
        Assert.DoesNotContain("<script>customer</script>", html);
    }
}
