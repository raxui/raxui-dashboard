using raxui_dashboard.Models.Reporting;

namespace raxui_dashboard.Features.Store.Models;

public sealed record StoreView(
    string Period,
    string Bucket,                              // day, week or month
    IReadOnlyList<DateOnly> Buckets,
    IReadOnlyList<StoreKpi> Kpis,
    IReadOnlyList<double> Sales,
    IReadOnlyList<double> Visits,
    IReadOnlyList<(string Source, double Visits)> Sources,
    IReadOnlyList<ProductSale> TopProducts,
    IReadOnlyList<(string Category, double Share)> Categories,
    (int InStock, int Low, int Out) Stock,
    IReadOnlyList<StockItem> LowStock,
    double Rating, int Reviews, IReadOnlyList<int> RatingCounts,   // RatingCounts[0] = 5 stars
    IReadOnlyList<string> Reviewers,
    IReadOnlyList<Order> Orders);
