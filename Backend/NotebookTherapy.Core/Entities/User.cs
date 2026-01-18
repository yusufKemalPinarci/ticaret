namespace NotebookTherapy.Core.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = "Customer";
    public bool IsEmailVerified { get; set; } = false;
    public bool IsLocked { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEndUtc { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Turkish Government / Legal Fields
    public bool IsCorporate { get; set; } = false;
    public string? TcKimlikNo { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? CompanyName { get; set; }
    public bool KvkkApproved { get; set; } = false;
    public DateTime? KvkkApprovedDate { get; set; }

    // Navigation Properties
    public List<Cart> Carts { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
