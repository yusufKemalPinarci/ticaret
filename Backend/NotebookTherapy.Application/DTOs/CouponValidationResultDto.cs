namespace NotebookTherapy.Application.DTOs;

public class CouponValidationResultDto
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public decimal DiscountAmount { get; set; }
    public CouponDto? Coupon { get; set; }
}
