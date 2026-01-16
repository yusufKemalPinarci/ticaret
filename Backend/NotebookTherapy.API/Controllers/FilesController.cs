using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NotebookTherapy.Core.Interfaces;
using System;
using System.IO;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _storage;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public FilesController(IFileStorageService storage)
    {
        _storage = storage;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Dosya yüklenmedi.");
        }

        var extension = Path.GetExtension(file.FileName);
        var safeExtension = string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.ToLowerInvariant();
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{safeExtension}";
        var relativePath = Path.Combine("uploads", "products", fileName).Replace("\\", "/");

        await using var stream = file.OpenReadStream();
        await _storage.SaveAsync(stream, relativePath, file.ContentType ?? "application/octet-stream", cancellationToken);

        var url = _storage.GetSignedUrl(relativePath);
        return Ok(new { path = relativePath, url });
    }

    [HttpGet("{**path}")]
    public async Task<IActionResult> Get(string path, [FromQuery] long exp, [FromQuery] string sig, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(sig) || exp <= 0)
        {
            return BadRequest();
        }

        if (!_storage.ValidateSignature(path, exp, sig))
        {
            return Forbid();
        }

        var stream = await _storage.OpenReadAsync(path, cancellationToken);
        if (stream == null)
        {
            return NotFound();
        }

        if (!_contentTypeProvider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(stream, contentType, enableRangeProcessing: true);
    }
}
