namespace NotebookTherapy.Application.DTOs;

public class CreateShippingRateDto
{
    public string Region { get; set; } = string.Empty;
    public decimal WeightFrom { get; set; }
    public decimal WeightTo { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "usd";
}
