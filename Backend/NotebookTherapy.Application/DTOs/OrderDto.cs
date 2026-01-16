namespace NotebookTherapy.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public int? CouponId { get; set; }
    public string? CouponCode { get; set; }
    public string? ShippingRegion { get; set; }
    public decimal? ShippingWeight { get; set; }
    public string? TaxRegion { get; set; }
    public string? InvoiceUrl { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentProvider { get; set; }
    public string? PaymentIntentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool RefundRequested { get; set; }
    public bool Refunded { get; set; }
    public string? AdminNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public int? ProductVariantId { get; set; }
    public string? VariantSku { get; set; }
    public string? VariantColor { get; set; }
    public string? VariantSize { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? TaxAmount { get; set; }
}
