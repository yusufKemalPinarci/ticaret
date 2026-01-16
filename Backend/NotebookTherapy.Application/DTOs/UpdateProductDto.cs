namespace NotebookTherapy.Application.DTOs;

public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal? Weight { get; set; }
    public string? ImageUrl { get; set; }
    public int? Stock { get; set; }
    public string? SKU { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsBackInStock { get; set; }
    public string? Collection { get; set; }
    public int? CategoryId { get; set; }
}
