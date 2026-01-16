namespace NotebookTherapy.Core.Entities;

public class Cart : BaseEntity
{
    public int? UserId { get; set; }
    public string? SessionId { get; set; } // For guest users
    public decimal TotalAmount { get; set; }
    
    // Navigation Properties
    public User? User { get; set; }
    public List<CartItem> Items { get; set; } = new();
}
