using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace NotebookTherapy.Infrastructure.Services;

public interface IJwtService
{
    string GenerateAccessToken(string email, string role, int userId, TimeSpan? lifetime = null);
    string GenerateAccessToken(Core.Entities.User user, TimeSpan? lifetime = null);
    string GenerateRefreshToken(int size = 64);
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(string email, string role, int userId, TimeSpan? lifetime = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var expiresIn = lifetime ?? TimeSpan.FromMinutes(30);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(expiresIn),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateAccessToken(Core.Entities.User user, TimeSpan? lifetime = null)
    {
        return GenerateAccessToken(user.Email, user.Role, user.Id, lifetime);
    }

    public string GenerateRefreshToken(int size = 64)
    {
        var bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
