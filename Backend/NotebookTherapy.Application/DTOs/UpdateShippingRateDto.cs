namespace NotebookTherapy.Application.DTOs;

public class UpdateShippingRateDto
{
    public string? Region { get; set; }
    public decimal? WeightFrom { get; set; }
    public decimal? WeightTo { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
}
