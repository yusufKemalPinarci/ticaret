namespace NotebookTherapy.Core.Entities;

public class TaxRate : BaseEntity
{
    public string Region { get; set; } = string.Empty;
    public decimal RatePercent { get; set; }
}