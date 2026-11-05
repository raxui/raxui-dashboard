using System.ComponentModel.DataAnnotations;
using raxui_dashboard.Services.Localization;

namespace raxui_dashboard.Features.Customers.Models;

public sealed class CustomerDraft(Ui l) : IValidatableObject
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Country { get; set; } = "Iraq";
    public string Plan { get; set; } = "Pro";
    public string Status { get; set; } = "Active";
    public bool Trial { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(Name)) yield return new(l["Enter the customer's name."], [nameof(Name)]);
        if (!new EmailAddressAttribute().IsValid(Email) || string.IsNullOrWhiteSpace(Email))
            yield return new(l["Enter a valid email address."], [nameof(Email)]);
    }
}
