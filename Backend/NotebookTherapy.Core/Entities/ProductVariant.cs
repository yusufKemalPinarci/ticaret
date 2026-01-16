namespace NotebookTherapy.Core.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Size { get; set; }
    public decimal Price { get; set; }
    public decimal? Weight { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;

    public Product Product { get; set; } = null!;
    public List<CartItem> CartItems { get; set; } = new();
    public List<OrderItem> OrderItems { get; set; } = new();
    public List<StockReservation> StockReservations { get; set; } = new();
}
