using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code);
}