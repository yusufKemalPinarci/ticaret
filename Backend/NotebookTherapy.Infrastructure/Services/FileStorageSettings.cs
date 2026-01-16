namespace NotebookTherapy.Infrastructure.Services;

public class FileStorageSettings
{
    public string RootPath { get; set; } = "storage";
    public string PublicBaseUrl { get; set; } = string.Empty;
    public string SigningKey { get; set; } = "local-dev-signing-key-change-me";
    public int SignedUrlTtlMinutes { get; set; } = 60 * 24 * 7; // default 7 days
}
