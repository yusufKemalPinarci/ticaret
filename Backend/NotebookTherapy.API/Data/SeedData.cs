using Microsoft.EntityFrameworkCore;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Infrastructure.Data;

namespace NotebookTherapy.API.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Kupa & Termos", Slug = "kupa-termos", DisplayOrder = 1 },
                new Category { Name = "Dekor & Mum", Slug = "dekor-mum", DisplayOrder = 2 },
                new Category { Name = "Kisiye Ozel", Slug = "kisiye-ozel", DisplayOrder = 3 },
                new Category { Name = "Ofis & Defter", Slug = "ofis-defter", DisplayOrder = 4 },
                new Category { Name = "Taki & Aksesuar", Slug = "taki-aksesuar", DisplayOrder = 5 },
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // Products
        if (!await context.Products.AnyAsync())
        {
            var mugsCategory = await context.Categories.FirstAsync(c => c.Name == "Kupa & Termos");
            var decorCategory = await context.Categories.FirstAsync(c => c.Name == "Dekor & Mum");
            var customCategory = await context.Categories.FirstAsync(c => c.Name == "Kisiye Ozel");
            var officeCategory = await context.Categories.FirstAsync(c => c.Name == "Ofis & Defter");
            var jewelryCategory = await context.Categories.FirstAsync(c => c.Name == "Taki & Aksesuar");

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Çift Katmanlı Isı Korumalı Termos Kupa",
                    Description = "Paslanmaz çelik, sızdırmaz kapak, 500 ml. Yolculuk ve ofis için ideal hediyelik.",
                    Price = 32.90m,
                    DiscountPrice = 27.90m,
                    ImageUrl = "https://images.unsplash.com/photo-1526404800669-8b2012e1f1aa?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1526404800669-8b2012e1f1aa?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 80,
                    SKU = "GFT-TERMOS-001",
                    IsFeatured = true,
                    IsNew = true,
                    Collection = "Gunluk Hediye",
                    Category = mugsCategory
                },
                new Product
                {
                    Name = "El Yapımı Lavanta Kokulu Soya Mumu",
                    Description = "Doğal soya mumu, pamuk fitil, 40 saat yanma süresi. Şık cam kavanozlu hediye.",
                    Price = 18.50m,
                    ImageUrl = "https://images.unsplash.com/photo-1512250796-9d8a5d7cc696?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1512250796-9d8a5d7cc696?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1504198453319-5ce911bafcde?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 60,
                    SKU = "GFT-MUM-002",
                    IsFeatured = true,
                    IsBackInStock = true,
                    Collection = "Rahatlatan Hediye",
                    Category = decorCategory
                },
                new Product
                {
                    Name = "Kişiye Özel İsim Baskılı Defter",
                    Description = "90 gsm krem sayfa, sert kapak, isim baskılı kişiye özel hediye defter.",
                    Price = 22.90m,
                    ImageUrl = "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1473186505569-9c61870c11f9?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 120,
                    SKU = "GFT-DEFTER-003",
                    IsNew = true,
                    Collection = "Kisiye Ozel",
                    Category = officeCategory
                },
                new Product
                {
                    Name = "Minimal Gümüş Kaplama Bileklik",
                    Description = "Ayaralı, nikel içermez, hediye kutulu ince bileklik.",
                    Price = 28.75m,
                    ImageUrl = "https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1522312346375-d1a52e2b99b3?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1520962918287-7448c2878f65?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 45,
                    SKU = "GFT-TAKI-004",
                    IsFeatured = true,
                    Collection = "Ozel Gun",
                    Category = jewelryCategory
                },
                new Product
                {
                    Name = "Kişiye Özel Fotoğraflı Kupa",
                    Description = "Sublimasyon baskı, 330 ml seramik kupa. Hediye notu eklenebilir.",
                    Price = 15.90m,
                    ImageUrl = "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1521572267360-ee0c2909d518?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1521017432531-fbd92d768814?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 150,
                    SKU = "GFT-KUPA-005",
                    IsBackInStock = true,
                    Collection = "Kisiye Ozel",
                    Category = customCategory
                },
                new Product
                {
                    Name = "Rustik Ahşap Fotoğraf Çerçevesi 10x15",
                    Description = "Doğal ahşap görünüm, masa veya duvar kullanımı için çok yönlü hediye.",
                    Price = 14.40m,
                    ImageUrl = "https://images.unsplash.com/photo-1523419400524-fc1e0a1a3c8e?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1523419400524-fc1e0a1a3c8e?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1505692794403-34d4982c3825?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 90,
                    SKU = "GFT-DEKOR-006",
                    Collection = "Anilar",
                    Category = decorCategory
                },
                new Product
                {
                    Name = "Ofis Starter Seti (Defter + Kalem + Masa Altlığı)",
                    Description = "Hediye kutusunda minimal set: sert kapak defter, jel kalem, keçe masa altlığı.",
                    Price = 39.90m,
                    ImageUrl = "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1489515217757-5fd1be406fef?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 70,
                    SKU = "GFT-OFIS-007",
                    IsFeatured = true,
                    Collection = "Ofise Donus",
                    Category = officeCategory
                },
                new Product
                {
                    Name = "Kokulu Taşlı Hediye Kutusu (3'lü)",
                    Description = "Üç farklı koku, kurdeleli hediye kutusu, dekoratif ve ferahlatıcı.",
                    Price = 19.80m,
                    ImageUrl = "https://images.unsplash.com/photo-1519681393784-d120267933ba?auto=format&fit=crop&w=800&q=80",
                    ImageUrls = new List<string>
                    {
                        "https://images.unsplash.com/photo-1519681393784-d120267933ba?auto=format&fit=crop&w=800&q=80",
                        "https://images.unsplash.com/photo-1489515217757-5fd1be406fef?auto=format&fit=crop&w=800&q=80"
                    },
                    Stock = 55,
                    SKU = "GFT-DEKOR-008",
                    IsNew = true,
                    Collection = "Hediye Kutusu",
                    Category = decorCategory
                },
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        // Shipping rates (TR region, weights in kg)
        if (!await context.ShippingRates.AnyAsync())
        {
            var rates = new List<ShippingRate>
            {
                new ShippingRate { Region = "TR", WeightFrom = 0m, WeightTo = 1m, Price = 39.90m, Currency = "try" },
                new ShippingRate { Region = "TR", WeightFrom = 1m, WeightTo = 3m, Price = 59.90m, Currency = "try" },
                new ShippingRate { Region = "TR", WeightFrom = 3m, WeightTo = 10m, Price = 89.90m, Currency = "try" },
            };
            await context.ShippingRates.AddRangeAsync(rates);
            await context.SaveChangesAsync();
        }

        // Tax rates
        if (!await context.TaxRates.AnyAsync())
        {
            var taxes = new List<TaxRate>
            {
                new TaxRate { Region = "TR", RatePercent = 10m }
            };
            await context.TaxRates.AddRangeAsync(taxes);
            await context.SaveChangesAsync();
        }

        // Coupons
        if (!await context.Coupons.AnyAsync())
        {
            var now = DateTime.UtcNow;
            var coupons = new List<Coupon>
            {
                new Coupon
                {
                    Code = "HOSGELDIN10",
                    Description = "Yeni üyelere %10 indirim",
                    DiscountType = "Percent",
                    Amount = 10m,
                    MinOrderAmount = 100m,
                    StartsAt = now.AddDays(-7),
                    ExpiresAt = now.AddMonths(3),
                    IsActive = true
                },
                new Coupon
                {
                    Code = "SEVGILILER15",
                    Description = "Sevgililer Günü'ne özel %15",
                    DiscountType = "Percent",
                    Amount = 15m,
                    MinOrderAmount = 150m,
                    StartsAt = now.AddDays(-14),
                    ExpiresAt = now.AddMonths(1),
                    IsActive = true
                },
                new Coupon
                {
                    Code = "KUTU50",
                    Description = "Hediye kutularında 50 TL indirim",
                    DiscountType = "Fixed",
                    Amount = 50m,
                    MinOrderAmount = 200m,
                    StartsAt = now.AddDays(-30),
                    ExpiresAt = now.AddMonths(2),
                    IsActive = true
                }
            };
            await context.Coupons.AddRangeAsync(coupons);
            await context.SaveChangesAsync();
        }

        // Admin user
        if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            var admin = new User
            {
                Email = "admin@local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsEmailVerified = true
            };
            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
