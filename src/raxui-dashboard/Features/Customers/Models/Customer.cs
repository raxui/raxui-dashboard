namespace raxui_dashboard.Features.Customers.Models;

public sealed record Customer(string Id, string Name, string Email, string Country, string Plan, string Status, int Orders, decimal Spent, DateOnly LastOrder);
