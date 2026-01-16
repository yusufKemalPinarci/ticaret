using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.Infrastructure.Repositories;

public class StockReservationRepository : Repository<StockReservation>, IStockReservationRepository
{
    public StockReservationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<int> GetActiveReservedQuantityAsync(int productId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(r => r.ProductId == productId && !r.IsDeleted && r.Status == "Reserved" && r.ExpiresAt > now)
            .SumAsync(r => r.Quantity);
    }

    public async Task<int> GetActiveReservedQuantityAsync(int productId, int? variantId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(r => r.ProductId == productId && r.ProductVariantId == variantId && !r.IsDeleted && r.Status == "Reserved" && r.ExpiresAt > now)
            .SumAsync(r => r.Quantity);
    }

    public async Task<IReadOnlyList<StockReservation>> GetActiveByUserAndKeyAsync(int userId, string idempotencyKey)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(r => r.UserId == userId && r.IdempotencyKey == idempotencyKey && !r.IsDeleted && r.Status == "Reserved" && r.ExpiresAt > now)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<StockReservation>> GetByOrderIdAsync(int orderId)
    {
        return await _dbSet
            .Where(r => r.OrderId == orderId && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<StockReservation>> GetExpiredReservationsAsync(DateTime utcNow)
    {
        return await _dbSet
            .Where(r => r.Status == "Reserved" && r.ExpiresAt <= utcNow && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task ReleaseRangeAsync(IEnumerable<StockReservation> reservations)
    {
        foreach (var reservation in reservations)
        {
            reservation.Status = "Released";
            reservation.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(reservation);
        }
        await Task.CompletedTask;
    }
}
