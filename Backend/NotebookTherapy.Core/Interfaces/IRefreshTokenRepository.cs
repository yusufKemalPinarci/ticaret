using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeAsync(RefreshToken token, string? revokedByIp = null, string? replacedBy = null);
    Task RevokeAllForUserAsync(int userId, string? revokedByIp = null);
    Task<List<RefreshToken>> GetByUserAsync(int userId);
}
