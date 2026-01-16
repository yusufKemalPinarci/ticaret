namespace NotebookTherapy.Core.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public int? CouponId { get; set; }
    public string? CouponCode { get; set; }
    public string? IdempotencyKey { get; set; }
    public string PaymentStatus { get; set; } = "Pending"; // Pending, RequiresAction, Succeeded, Failed
    public string? PaymentProvider { get; set; }
    public string? PaymentIntentId { get; set; }
    public int PaymentRetryCount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
    public string? TrackingNumber { get; set; }
    public string? ShippingRegion { get; set; }
    public decimal? ShippingWeight { get; set; }
    public string? TaxRegion { get; set; }
    public string? InvoiceUrl { get; set; }
    public decimal RefundAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string BillingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool RefundRequested { get; set; }
    public bool Refunded { get; set; }
    public string? AdminNote { get; set; }
    
    // Navigation Properties
    public User User { get; set; } = null!;
    public List<OrderItem> Items { get; set; } = new();
    public Coupon? Coupon { get; set; }
}
