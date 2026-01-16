using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IStockReservationRepository : IRepository<StockReservation>
{
    Task<int> GetActiveReservedQuantityAsync(int productId);
    Task<int> GetActiveReservedQuantityAsync(int productId, int? variantId);
    Task<IReadOnlyList<StockReservation>> GetActiveByUserAndKeyAsync(int userId, string idempotencyKey);
    Task<IReadOnlyList<StockReservation>> GetByOrderIdAsync(int orderId);
    Task<IReadOnlyList<StockReservation>> GetExpiredReservationsAsync(DateTime utcNow);
    Task ReleaseRangeAsync(IEnumerable<StockReservation> reservations);
}
