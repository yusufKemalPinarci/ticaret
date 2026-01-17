namespace NotebookTherapy.Application.DTOs;

public class WishlistDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public decimal? ProductDiscountPrice { get; set; }
}
