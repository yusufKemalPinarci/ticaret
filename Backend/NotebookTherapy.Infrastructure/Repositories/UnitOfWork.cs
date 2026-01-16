using Microsoft.EntityFrameworkCore.Storage;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Infrastructure.Data;
using NotebookTherapy.Infrastructure.Repositories;

namespace NotebookTherapy.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IProductRepository? _products;
    private ICategoryRepository? _categories;
    private ICartRepository? _carts;
    private IOrderRepository? _orders;
    private IUserRepository? _users;
    private ICouponRepository? _coupons;
    private IStockReservationRepository? _stockReservations;
    private ICheckoutSessionRepository? _checkoutSessions;
    private ICouponRedemptionRepository? _couponRedemptions;
    private IProductVariantRepository? _productVariants;
    private IShippingRateRepository? _shippingRates;
    private ITaxRateRepository? _taxRates;
    private IRefreshTokenRepository? _refreshTokens;
    private IAuditLogRepository? _auditLogs;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products =>
        _products ??= new ProductRepository(_context);

    public ICategoryRepository Categories =>
        _categories ??= new CategoryRepository(_context);

    public ICartRepository Carts =>
        _carts ??= new CartRepository(_context);

    public IOrderRepository Orders =>
        _orders ??= new OrderRepository(_context);

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public ICouponRepository Coupons =>
        _coupons ??= new CouponRepository(_context);

    public IStockReservationRepository StockReservations =>
        _stockReservations ??= new StockReservationRepository(_context);

    public ICheckoutSessionRepository CheckoutSessions =>
        _checkoutSessions ??= new CheckoutSessionRepository(_context);

    public ICouponRedemptionRepository CouponRedemptions =>
        _couponRedemptions ??= new CouponRedemptionRepository(_context);

    public IProductVariantRepository ProductVariants =>
        _productVariants ??= new ProductVariantRepository(_context);

    public IShippingRateRepository ShippingRates =>
        _shippingRates ??= new ShippingRateRepository(_context);

    public ITaxRateRepository TaxRates =>
        _taxRates ??= new TaxRateRepository(_context);

    public IRefreshTokenRepository RefreshTokens =>
        _refreshTokens ??= new RefreshTokenRepository(_context);

    public IAuditLogRepository AuditLogs =>
        _auditLogs ??= new AuditLogRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
