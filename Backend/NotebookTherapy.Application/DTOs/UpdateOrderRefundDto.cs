namespace NotebookTherapy.Application.DTOs;

public class UpdateOrderRefundDto
{
    public bool? RefundRequested { get; set; }
    public bool? Refunded { get; set; }
    public string? AdminNote { get; set; }
    public decimal? RefundAmount { get; set; }
}
