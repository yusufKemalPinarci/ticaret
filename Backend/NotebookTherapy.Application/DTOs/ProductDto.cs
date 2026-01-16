namespace NotebookTherapy.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal? Weight { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    public int Stock { get; set; }
    public string SKU { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public bool IsBackInStock { get; set; }
    public string Collection { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
