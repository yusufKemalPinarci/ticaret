namespace NotebookTherapy.Application.DTOs;

public class UpdateCouponDto
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? DiscountType { get; set; }
    public decimal? Amount { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? MaxUsageCount { get; set; }
    public bool? IsSingleUsePerUser { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool? IsActive { get; set; }
}
