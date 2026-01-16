using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<ShippingRate> ShippingRates { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }
    public DbSet<StockReservation> StockReservations { get; set; }
    public DbSet<CheckoutSession> CheckoutSessions { get; set; }
    public DbSet<CouponRedemption> CouponRedemptions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product Configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(18,3)");
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(100);
            entity.Property(e => e.Size).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(18,3)");
            entity.HasIndex(e => new { e.ProductId, e.SKU }).IsUnique();
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Variants)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FailedLoginCount);
            entity.Property(e => e.LockoutEndUtc);
            entity.Property(e => e.LastLoginAt);
        });

        // Cart Configuration
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasMany(e => e.Items)
                  .WithOne(i => i.Cart)
                  .HasForeignKey(i => i.CartId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItem Configuration
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.CartItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ProductVariant)
                .WithMany(v => v.CartItems)
                .HasForeignKey(e => e.ProductVariantId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Metadata).HasMaxLength(2000);
            entity.Property(e => e.IpAddress).HasMaxLength(100);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Order Configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingWeight).HasColumnType("decimal(18,3)");
            entity.Property(e => e.TaxRegion).HasMaxLength(100);
            entity.Property(e => e.InvoiceUrl).HasMaxLength(500);
            entity.HasMany(e => e.Items)
                  .WithOne(i => i.Order)
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Coupon)
                  .WithMany()
                  .HasForeignKey(e => e.CouponId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // OrderItem Configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ProductVariant)
                .WithMany(v => v.OrderItems)
                .HasForeignKey(e => e.ProductVariantId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Coupon Configuration
        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountType).HasMaxLength(20);
        });

        modelBuilder.Entity<StockReservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.HasIndex(e => new { e.UserId, e.IdempotencyKey });
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ProductVariant)
                .WithMany(v => v.StockReservations)
                .HasForeignKey(e => e.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Order)
                  .WithMany()
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ShippingRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Region).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WeightFrom).HasColumnType("decimal(18,3)");
            entity.Property(e => e.WeightTo).HasColumnType("decimal(18,3)");
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.HasIndex(e => new { e.Region, e.WeightFrom, e.WeightTo }).IsUnique();
        });

        modelBuilder.Entity<TaxRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Region).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RatePercent).HasColumnType("decimal(5,2)");
            entity.HasIndex(e => e.Region).IsUnique();
        });

        modelBuilder.Entity<CheckoutSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdempotencyKey).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => new { e.UserId, e.IdempotencyKey }).IsUnique();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<CouponRedemption>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CouponId, e.UserId }).IsUnique();
            entity.HasIndex(e => new { e.CouponId, e.OrderId });
            entity.HasOne(e => e.Coupon)
                  .WithMany()
                  .HasForeignKey(e => e.CouponId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Order)
                  .WithMany()
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

          modelBuilder.Entity<RefreshToken>(entity =>
          {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(512);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
          });
    }
}
