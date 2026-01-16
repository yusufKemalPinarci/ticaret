namespace NotebookTherapy.Application.DTOs;

public class TaxRateDto
{
    public int Id { get; set; }
    public string Region { get; set; } = string.Empty;
    public decimal RatePercent { get; set; }
}
