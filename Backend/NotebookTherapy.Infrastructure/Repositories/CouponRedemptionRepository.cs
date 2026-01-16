using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class CouponRedemptionRepository : Repository<CouponRedemption>, ICouponRedemptionRepository
{
    public CouponRedemptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<CouponRedemption?> GetByUserAndCouponAsync(int userId, int couponId)
    {
        return await _dbSet
            .Where(r => r.UserId == userId && r.CouponId == couponId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<CouponRedemption?> GetByOrderAndCouponAsync(int orderId, int couponId)
    {
        return await _dbSet
            .Where(r => r.OrderId == orderId && r.CouponId == couponId && !r.IsDeleted)
            .FirstOrDefaultAsync();
    }
}
