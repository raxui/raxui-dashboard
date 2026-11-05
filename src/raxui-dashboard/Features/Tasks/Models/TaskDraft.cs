using System.ComponentModel.DataAnnotations;
using raxui_dashboard.Services.Localization;

namespace raxui_dashboard.Features.Tasks.Models;

public sealed class TaskDraft(Ui l, DateOnly today) : IValidatableObject
{
    public string Title { get; set; } = "";
    public string Column { get; set; } = "todo";
    public string Tag { get; set; } = "Operations";
    public string Priority { get; set; } = "Medium";
    public string Assignee { get; set; } = "Noura Al-Qahtani";
    public DateOnly? Due { get; set; } = today.AddDays(7);

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(Title)) yield return new(l["Give the task a title."], [nameof(Title)]);
        else if (Title.Trim().Length > 80) yield return new(l["Keep the title under 80 characters."], [nameof(Title)]);
    }
}
