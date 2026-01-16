namespace NotebookTherapy.Application.DTOs;

public class PaymentIntentDto
{
    public string ClientSecret { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
    public string Provider { get; set; } = "Stripe";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
}
