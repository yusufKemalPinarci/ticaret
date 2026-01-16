using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class CheckoutSessionRepository : Repository<CheckoutSession>, ICheckoutSessionRepository
{
    public CheckoutSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<CheckoutSession?> GetByUserAndKeyAsync(int userId, string idempotencyKey)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && s.IdempotencyKey == idempotencyKey && !s.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<CheckoutSession?> GetByOrderIdAsync(int orderId)
    {
        return await _dbSet
            .Where(s => s.OrderId == orderId && !s.IsDeleted)
            .FirstOrDefaultAsync();
    }
}
