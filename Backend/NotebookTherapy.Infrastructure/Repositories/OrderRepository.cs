using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(o => o.UserId == userId && !o.IsDeleted)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _dbSet
            .Where(o => o.OrderNumber == orderNumber && !o.IsDeleted)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync();
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _dbSet
            .Where(o => o.Id == orderId && !o.IsDeleted)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Category)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync();
    }

    public async Task<Order?> GetByPaymentIntentIdAsync(string paymentIntentId)
    {
        return await _dbSet
            .Where(o => o.PaymentIntentId == paymentIntentId && !o.IsDeleted)
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync();
    }
}
