namespace NotebookTherapy.Core.Entities;

public class Product : BaseEntity
{
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
    public string Collection { get; set; } = string.Empty; // Tsuki, Hinoki, etc.
    
    // Navigation Properties
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public List<CartItem> CartItems { get; set; } = new();
    public List<OrderItem> OrderItems { get; set; } = new();
    public List<ProductVariant> Variants { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}
