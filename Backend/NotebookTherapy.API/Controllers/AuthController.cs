using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Services;
using System.Text;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AuthController> _logger;
    private readonly IEmailService _emailService;
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);
    private const int RateLimitCount = 20;
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromHours(1);
    private static readonly TimeSpan EmailVerifyTokenLifetime = TimeSpan.FromDays(1);

    public AuthController(IUnitOfWork unitOfWork, IJwtService jwtService, IMemoryCache cache, ILogger<AuthController> logger, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _cache = cache;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email)) return BadRequest(new { message = "Email gerekli" });

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null || user.IsDeleted || !user.IsActive)
        {
            return Ok(); // gizlilik için her zaman 200
        }

        var token = GenerateSimpleToken();
        var cacheKey = GetResetCacheKey(user.Email);
        _cache.Set(cacheKey, token, ResetTokenLifetime);

        try
        {
            await _emailService.SendAsync(user.Email, "Şifre Sıfırlama", $"Şifrenizi sıfırlamak için kodunuz: <b>{token}</b>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
        }

        _logger.LogInformation("Password reset token generated for {Email}: {Token}", user.Email, token);

        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest(new { message = "Eksik bilgi" });
        }

        var cacheKey = GetResetCacheKey(request.Email);
        var cached = _cache.Get<string>(cacheKey);
        if (cached == null || !string.Equals(cached, request.Token, StringComparison.Ordinal))
        {
            return BadRequest(new { message = "Geçersiz veya süresi dolmuş token" });
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null) return BadRequest(new { message = "Kullanıcı bulunamadı" });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.RefreshTokens.RevokeAllForUserAsync(user.Id, GetIp());
        await _unitOfWork.SaveChangesAsync();
        _cache.Remove(cacheKey);

        await LogAuditAsync(user.Id, "reset-password", null);
        return NoContent();
    }

    [HttpPost("verify-email/send")]
    public async Task<ActionResult> SendVerifyEmail([FromBody] VerifyEmailRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email)) return BadRequest(new { message = "Email gerekli" });

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null || user.IsDeleted || !user.IsActive)
        {
            return Ok();
        }

        var token = GenerateSimpleToken();
        var cacheKey = GetVerifyCacheKey(user.Email);
        _cache.Set(cacheKey, token, EmailVerifyTokenLifetime);

        try
        {
            await _emailService.SendAsync(user.Email, "E-posta Doğrulama", $"E-postanızı doğrulamak için kodunuz: <b>{token}</b>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email}", user.Email);
        }

        _logger.LogInformation("Email verify token generated for {Email}: {Token}", user.Email, token);
        return Ok();
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequest(new { message = "Eksik bilgi" });
        }

        var cacheKey = GetVerifyCacheKey(request.Email);
        var cached = _cache.Get<string>(cacheKey);
        if (cached == null || !string.Equals(cached, request.Token, StringComparison.Ordinal))
        {
            return BadRequest(new { message = "Geçersiz veya süresi dolmuş token" });
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null) return BadRequest(new { message = "Kullanıcı bulunamadı" });

        user.IsEmailVerified = true;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        _cache.Remove(cacheKey);
        await LogAuditAsync(user.Id, "verify-email", null);
        return NoContent();
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (IsRateLimited("register")) return TooManyRequests();

        if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Role = "Customer"
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user, AccessTokenLifetime);
        var refreshToken = CreateRefreshToken(user.Id);
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        await LogAuditAsync(user.Id, "register", null);

        return Ok(BuildAuthResponse(user, accessToken, refreshToken));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (IsRateLimited("login")) return TooManyRequests();

        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
        
        if (user == null || user.IsLocked && user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value > DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Account locked. Please try later." });
        }

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            if (user != null)
            {
                user.FailedLoginCount += 1;
                if (user.FailedLoginCount >= MaxFailedAttempts)
                {
                    user.IsLocked = true;
                    user.LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
                }
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            return Unauthorized(new { message = "Invalid email or password" });
        }

        user.FailedLoginCount = 0;
        user.IsLocked = false;
        user.LockoutEndUtc = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);

        await _unitOfWork.RefreshTokens.RevokeAllForUserAsync(user.Id, GetIp());
        var accessToken = _jwtService.GenerateAccessToken(user, AccessTokenLifetime);
        var refreshToken = CreateRefreshToken(user.Id);
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        await LogAuditAsync(user.Id, "login", null);

        return Ok(BuildAuthResponse(user, accessToken, refreshToken));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken)) return BadRequest();

        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);
        if (token == null || !token.IsActive)
            return Unauthorized(new { message = "Invalid refresh token" });

        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);
        if (user == null || user.IsDeleted || !user.IsActive)
            return Unauthorized(new { message = "Invalid user" });

        await _unitOfWork.RefreshTokens.RevokeAsync(token, GetIp());

        var newRefresh = CreateRefreshToken(user.Id);
        newRefresh.ReplacedByToken = null;
        token.ReplacedByToken = newRefresh.Token;

        var accessToken = _jwtService.GenerateAccessToken(user, AccessTokenLifetime);

        await _unitOfWork.RefreshTokens.AddAsync(newRefresh);
        await _unitOfWork.RefreshTokens.UpdateAsync(token);
        await _unitOfWork.SaveChangesAsync();

        await LogAuditAsync(user.Id, "refresh", null);

        return Ok(BuildAuthResponse(user, accessToken, newRefresh));
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] RefreshRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken)) return BadRequest(new { message = "Refresh token gerekli" });
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);
        if (token != null)
        {
            await _unitOfWork.RefreshTokens.RevokeAsync(token, GetIp());
            await _unitOfWork.SaveChangesAsync();
            await LogAuditAsync(token.UserId, "logout", null);
        }
        return NoContent();
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetSessions()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var tokens = await _unitOfWork.RefreshTokens.GetByUserAsync(userId.Value);
        var dtos = tokens.Select(t => new SessionDto
        {
            Token = t.Token,
            ExpiresAt = t.ExpiresAt,
            RevokedAt = t.RevokedAt,
            CreatedAt = t.CreatedAt,
            CreatedByIp = t.CreatedByIp,
            RevokedByIp = t.RevokedByIp,
            ReplacedByToken = t.ReplacedByToken,
            IsActive = t.IsActive
        });

        return Ok(dtos);
    }

    [Authorize]
    [HttpPost("sessions/revoke")]
    public async Task<ActionResult> RevokeSession([FromBody] RevokeSessionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Token)) return BadRequest();
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.Token);
        if (token == null || token.UserId != userId.Value) return NotFound();

        await _unitOfWork.RefreshTokens.RevokeAsync(token, GetIp());
        await _unitOfWork.SaveChangesAsync();
        await LogAuditAsync(userId.Value, "session-revoke", null);
        return NoContent();
    }

    private RefreshToken CreateRefreshToken(int userId)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = GetIp()
        };
    }

    private AuthResponseDto BuildAuthResponse(User user, string accessToken, RefreshToken refreshToken)
    {
        var accessExpires = DateTime.UtcNow.Add(AccessTokenLifetime);
        return new AuthResponseDto
        {
            Token = accessToken,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = accessExpires,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role
        };
    }

    private int? GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var id) ? id : null;
    }

    private string? GetIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static string GenerateSimpleToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string GetResetCacheKey(string email) => $"pwd-reset:{email.ToLowerInvariant()}";
    private static string GetVerifyCacheKey(string email) => $"verify-email:{email.ToLowerInvariant()}";

    private bool IsRateLimited(string keyPrefix)
    {
        var key = $"rl:{keyPrefix}:{GetIp()}";
        var count = _cache.GetOrCreate<int>(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = RateLimitWindow;
            return 0;
        });
        count++;
        _cache.Set(key, count, RateLimitWindow);
        return count > RateLimitCount;
    }

    private ActionResult TooManyRequests()
    {
        return StatusCode(StatusCodes.Status429TooManyRequests, new { message = "Too many requests. Please slow down." });
    }

    private async Task LogAuditAsync(int? userId, string action, string? metadata)
    {
        try
        {
            var entry = new AuditLog
            {
                UserId = userId,
                Action = action,
                Metadata = metadata,
                IpAddress = GetIp()
            };
            await _unitOfWork.AuditLogs.AddAsync(entry);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit event {Action}", action);
        }
    }
}
