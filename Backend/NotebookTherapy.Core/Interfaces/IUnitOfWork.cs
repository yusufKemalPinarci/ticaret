namespace NotebookTherapy.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    IUserRepository Users { get; }
    ICouponRepository Coupons { get; }
    IStockReservationRepository StockReservations { get; }
    ICheckoutSessionRepository CheckoutSessions { get; }
    ICouponRedemptionRepository CouponRedemptions { get; }
    IProductVariantRepository ProductVariants { get; }
    IShippingRateRepository ShippingRates { get; }
    ITaxRateRepository TaxRates { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IAuditLogRepository AuditLogs { get; }
    IReviewRepository Reviews { get; }
    IWishlistRepository Wishlist { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task CommitAsync();
}
