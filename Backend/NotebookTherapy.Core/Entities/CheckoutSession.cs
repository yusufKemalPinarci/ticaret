namespace NotebookTherapy.Core.Entities;

public class CheckoutSession : BaseEntity
{
    public int UserId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public string? PaymentIntentId { get; set; }
    public decimal? TotalAmount { get; set; }
}
