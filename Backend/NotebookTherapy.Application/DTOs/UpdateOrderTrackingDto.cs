namespace NotebookTherapy.Application.DTOs;

public class UpdateOrderTrackingDto
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string? Status { get; set; } // optional: e.g., Shipped
}
