using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface ICouponRedemptionRepository : IRepository<CouponRedemption>
{
    Task<CouponRedemption?> GetByUserAndCouponAsync(int userId, int couponId);
    Task<CouponRedemption?> GetByOrderAndCouponAsync(int orderId, int couponId);
}
