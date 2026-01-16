using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string relativePath, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default);
    string GetSignedUrl(string relativePath, TimeSpan? ttl = null);
    bool ValidateSignature(string relativePath, long expiresUnixTimeSeconds, string signature);
}
