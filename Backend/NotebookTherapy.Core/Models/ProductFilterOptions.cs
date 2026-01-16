namespace NotebookTherapy.Core.Models;

public class ProductFilterOptions
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Collection { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsBackInStock { get; set; }
    public bool? InStockOnly { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? HasDiscount { get; set; }
    public string? SortBy { get; set; }
}
