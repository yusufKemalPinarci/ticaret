namespace NotebookTherapy.Core.Entities;

public class StockReservation : BaseEntity
{
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public int UserId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; } = "Reserved"; // Reserved, Committed, Released
    public int? OrderId { get; set; }

    public Product Product { get; set; } = null!;
    public Order? Order { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}
