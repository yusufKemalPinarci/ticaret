namespace NotebookTherapy.Core.Entities;

public class CouponRedemption : BaseEntity
{
    public int CouponId { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string? IdempotencyKey { get; set; }

    public Coupon Coupon { get; set; } = null!;
    public Order? Order { get; set; }
}
