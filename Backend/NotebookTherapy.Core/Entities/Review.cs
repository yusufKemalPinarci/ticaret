namespace NotebookTherapy.Core.Entities;

public class Review : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int Rating { get; set; } // 1 to 5
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = true; // Auto approve for now
}
