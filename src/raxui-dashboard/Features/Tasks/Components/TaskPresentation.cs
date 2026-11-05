namespace raxui_dashboard.Features.Tasks.Components;

public static class TaskPresentation
{
    public static string TagColor(string tag) => tag switch
    {
        "Design" => "primary", "Engineering" => "info", "Marketing" => "warning", "Finance" => "success", _ => "neutral",
    };

    public static string PriorityColor(string p) => p switch { "High" => "danger", "Medium" => "warning", _ => "neutral" };
}
