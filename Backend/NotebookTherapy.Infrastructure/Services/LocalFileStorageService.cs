using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private readonly string _publicBaseUrl;
    private readonly byte[] _signingKey;
    private readonly TimeSpan _defaultTtl;

    public LocalFileStorageService(IHostEnvironment env, IOptions<FileStorageSettings> options)
    {
        var settings = options.Value ?? new FileStorageSettings();
        var resolvedRoot = settings.RootPath;
        if (string.IsNullOrWhiteSpace(resolvedRoot))
        {
            resolvedRoot = "storage";
        }

        _rootPath = Path.GetFullPath(Path.IsPathRooted(resolvedRoot)
            ? resolvedRoot
            : Path.Combine(env.ContentRootPath, resolvedRoot));
        Directory.CreateDirectory(_rootPath);

        _publicBaseUrl = settings.PublicBaseUrl?.TrimEnd('/') ?? string.Empty;
        var signingKey = string.IsNullOrWhiteSpace(settings.SigningKey)
            ? "local-dev-signing-key-change-me"
            : settings.SigningKey;
        _signingKey = Encoding.UTF8.GetBytes(signingKey);

        var ttlMinutes = settings.SignedUrlTtlMinutes <= 0 ? 60 * 24 : settings.SignedUrlTtlMinutes;
        _defaultTtl = TimeSpan.FromMinutes(ttlMinutes);
    }

    public async Task<string> SaveAsync(Stream content, string relativePath, string contentType, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _ = contentType; // content type kept for parity with interface; local storage does not persist metadata

        var normalizedPath = NormalizePath(relativePath);
        var targetPath = Path.Combine(_rootPath, normalizedPath);
        var directory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (content.CanSeek)
        {
            content.Position = 0;
        }

        await using var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await content.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);

        return normalizedPath.Replace("\\", "/");
    }

    public Task<Stream?> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedPath = NormalizePath(relativePath);
            var targetPath = Path.Combine(_rootPath, normalizedPath);
            if (!File.Exists(targetPath))
            {
                return Task.FromResult<Stream?>(null);
            }

            var stream = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
            return Task.FromResult<Stream?>(stream);
        }
        catch
        {
            return Task.FromResult<Stream?>(null);
        }
    }

    public string GetSignedUrl(string relativePath, TimeSpan? ttl = null)
    {
        var normalizedPath = NormalizePath(relativePath);
        var expires = DateTimeOffset.UtcNow.Add(ttl ?? _defaultTtl).ToUnixTimeSeconds();
        var signature = ComputeSignature(normalizedPath, expires);
        var encodedPath = Uri.EscapeDataString(normalizedPath);

        var prefix = string.IsNullOrWhiteSpace(_publicBaseUrl) ? string.Empty : _publicBaseUrl;
        return $"{prefix}/files/{encodedPath}?exp={expires}&sig={signature}";
    }

    public bool ValidateSignature(string relativePath, long expiresUnixTimeSeconds, string signature)
    {
        if (expiresUnixTimeSeconds <= 0)
        {
            return false;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now > expiresUnixTimeSeconds)
        {
            return false;
        }

        try
        {
            var normalizedPath = NormalizePath(relativePath);
            var expected = ComputeSignature(normalizedPath, expiresUnixTimeSeconds);
            var providedBytes = Encoding.UTF8.GetBytes(signature);
            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            return CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
        }
        catch
        {
            return false;
        }
    }

    private string NormalizePath(string relativePath)
    {
        var cleaned = (relativePath ?? string.Empty).Replace("\\", "/").TrimStart('/');
        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new ArgumentException("Relative path cannot be empty", nameof(relativePath));
        }

        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, cleaned));
        if (!fullPath.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid storage path.");
        }

        return cleaned;
    }

    private string ComputeSignature(string relativePath, long expires)
    {
        var payload = Encoding.UTF8.GetBytes($"{relativePath}|{expires}");
        using var hmac = new HMACSHA256(_signingKey);
        var hash = hmac.ComputeHash(payload);
        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
