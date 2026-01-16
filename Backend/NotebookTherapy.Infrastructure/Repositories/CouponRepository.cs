using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class CouponRepository : Repository<Coupon>, ICouponRepository
{
    public CouponRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Coupon>> GetAllAsync()
    {
        return await _dbSet
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public override async Task<Coupon?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public override async Task<Coupon> AddAsync(Coupon entity)
    {
        entity.Code = entity.Code.Trim().ToUpperInvariant();
        return await base.AddAsync(entity);
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return await _dbSet
            .Where(c => c.Code == normalized && !c.IsDeleted)
            .FirstOrDefaultAsync();
    }
}
