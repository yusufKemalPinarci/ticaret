namespace NotebookTherapy.Application.DTOs;

public class UpdateUserAdminDto
{
    public string? Role { get; set; }
    public bool? IsLocked { get; set; }
    public bool? IsActive { get; set; }
    public bool? VerifyEmail { get; set; }
    public bool? IsCorporate { get; set; }
    public string? TcKimlikNo { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
    public string? CompanyName { get; set; }
    public bool? KvkkApproved { get; set; }
}
