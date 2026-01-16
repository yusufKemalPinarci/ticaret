namespace NotebookTherapy.Application.DTOs;

public class CreateTaxRateDto
{
    public string Region { get; set; } = string.Empty;
    public decimal RatePercent { get; set; }
}
