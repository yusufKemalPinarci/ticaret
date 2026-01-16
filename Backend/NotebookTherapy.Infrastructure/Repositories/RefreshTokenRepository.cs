using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.Token == token && !r.IsDeleted);
    }

    public async Task<List<RefreshToken>> GetByUserAsync(int userId)
    {
        return await _dbSet.Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task RevokeAsync(RefreshToken token, string? revokedByIp = null, string? replacedBy = null)
    {
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = revokedByIp;
        token.ReplacedByToken = replacedBy;
        _dbSet.Update(token);
        await Task.CompletedTask;
    }

    public async Task RevokeAllForUserAsync(int userId, string? revokedByIp = null)
    {
        var tokens = await _dbSet.Where(r => r.UserId == userId && r.RevokedAt == null && !r.IsDeleted).ToListAsync();
        foreach (var t in tokens)
        {
            t.RevokedAt = DateTime.UtcNow;
            t.RevokedByIp = revokedByIp;
        }
        if (tokens.Count > 0)
        {
            _dbSet.UpdateRange(tokens);
        }
    }
}
