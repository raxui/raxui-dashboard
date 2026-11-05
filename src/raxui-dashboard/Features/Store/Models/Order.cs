namespace raxui_dashboard.Features.Store.Models;

public sealed record Order(string Id, string Customer, string Email, string Channel, DateOnly Date, string Status, decimal Amount);
