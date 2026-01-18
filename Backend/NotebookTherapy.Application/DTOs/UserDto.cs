namespace NotebookTherapy.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; }
    public bool IsCorporate { get; set; }
    public string? TcKimlikNo { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? CompanyName { get; set; }
    public bool KvkkApproved { get; set; }
}
