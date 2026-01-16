namespace NotebookTherapy.Core.Entities;

public class ShippingRate : BaseEntity
{
    public string Region { get; set; } = string.Empty;
    public decimal WeightFrom { get; set; }
    public decimal WeightTo { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "usd";
}