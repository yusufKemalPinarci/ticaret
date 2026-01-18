namespace NotebookTherapy.Application.DTOs;

public class CheckoutDto
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string BillingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public string ShippingRegion { get; set; } = string.Empty;
    public decimal TotalWeight { get; set; }

    // Turkish Government / Legal Fields
    public bool IsCorporate { get; set; }
    public string? TcKimlikNo { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? CompanyName { get; set; }
    public bool KvkkApproved { get; set; }
}