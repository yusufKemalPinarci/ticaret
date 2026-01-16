namespace NotebookTherapy.Application.DTOs;

public class UpdateUserAdminDto
{
    public string? Role { get; set; }
    public bool? IsLocked { get; set; }
    public bool? IsActive { get; set; }
    public bool? VerifyEmail { get; set; }
}
