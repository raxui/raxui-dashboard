using System.ComponentModel.DataAnnotations;

namespace raxui_dashboard.Features.Profile.Models;

public sealed class ProfileDraft
{
    [Required(ErrorMessage = "Enter your name.")] public string Name { get; set; } = "Noura Al-Qahtani";
    [Required(ErrorMessage = "Enter your email."), EmailAddress(ErrorMessage = "Enter a valid email.")] public string Email { get; set; } = "noura@acme.co";
    public string Role { get; set; } = "Admin";
    [StringLength(160)] public string? Bio { get; set; } = "Head of e-commerce at Acme.";
}
