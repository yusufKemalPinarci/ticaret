namespace NotebookTherapy.Core.Entities;

public class AuditLog : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    public string? IpAddress { get; set; }

    public User? User { get; set; }
}
