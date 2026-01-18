using System;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Application.Features.Orders.Commands;
using System.Threading;
using System.Threading.Tasks;
using NotebookTherapy.Application.Features.Coupons;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Services;
using CartEntity = NotebookTherapy.Core.Entities.Cart;

namespace NotebookTherapy.Application.Features.Orders.Handlers;

public class OrderCommandHandlers :
    IRequestHandler<CreateOrderFromCartCommand, OrderDto?>,
    IRequestHandler<UpdateOrderStatusCommand, bool>,
    IRequestHandler<UpdateOrderRefundCommand, bool>,
    IRequestHandler<UpdateOrderNoteCommand, bool>,
    IRequestHandler<ApplyCouponCommand, bool>,
    IRequestHandler<UnapplyCouponCommand, bool>,
    IRequestHandler<CreatePaymentIntentCommand, PaymentIntentDto?>,
    IRequestHandler<UpdatePaymentStatusCommand, bool>,
    IRequestHandler<UpdateOrderTrackingCommand, bool>,
    IRequestHandler<RefundPaymentCommand, bool>,
    IRequestHandler<RequestRefundCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<OrderCommandHandlers> _logger;
    private readonly IMemoryCache _cache;
    private const string AllOrdersKey = "orders_all";

    public OrderCommandHandlers(IUnitOfWork uow, IMapper mapper, IPaymentService paymentService, IEmailService emailService, IInvoiceService invoiceService, ILogger<OrderCommandHandlers> logger, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _paymentService = paymentService;
        _emailService = emailService;
        _invoiceService = invoiceService;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return false;
        order.Status = request.Status;
        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{request.OrderId}");

        var subject = $"Sipariş durumunuz güncellendi - {order.OrderNumber}";
        var body = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişinizin durumu: {order.Status}.";
        await SendUserEmailAsync(order, subject, body);
        return true;
    }

    public async Task<OrderDto?> Handle(CreateOrderFromCartCommand request, CancellationToken cancellationToken)
    {
        var idempotencyKey = request.Checkout.IdempotencyKey?.Trim();
        if (string.IsNullOrWhiteSpace(idempotencyKey)) return null;

        var sessionId = request.SessionId ?? request.Checkout.SessionId?.Trim();
        var shippingRegion = request.Checkout.ShippingRegion?.Trim();
        if (!string.Equals(shippingRegion, "TR", StringComparison.OrdinalIgnoreCase)) return null;

        // Determine or create user (guest allowed)
        var userId = request.UserId;
        if (!userId.HasValue || userId.Value <= 0)
        {
            if (string.IsNullOrWhiteSpace(request.Checkout.Email)) return null;
            var existingUser = await _uow.Users.GetByEmailAsync(request.Checkout.Email.Trim());
            if (existingUser != null)
            {
                userId = existingUser.Id;
                // best-effort update names if empty
                var updated = false;
                if (string.IsNullOrWhiteSpace(existingUser.FirstName) && !string.IsNullOrWhiteSpace(request.Checkout.FirstName)) { existingUser.FirstName = request.Checkout.FirstName; updated = true; }
                if (string.IsNullOrWhiteSpace(existingUser.LastName) && !string.IsNullOrWhiteSpace(request.Checkout.LastName)) { existingUser.LastName = request.Checkout.LastName; updated = true; }
                if (updated)
                {
                    await _uow.Users.UpdateAsync(existingUser);
                    await _uow.SaveChangesAsync();
                }
            }
            else
            {
                var randomPassword = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N"));
                var newUser = new User
                {
                    Email = request.Checkout.Email.Trim(),
                    FirstName = request.Checkout.FirstName?.Trim() ?? string.Empty,
                    LastName = request.Checkout.LastName?.Trim() ?? string.Empty,
                    PasswordHash = randomPassword,
                    Role = "Customer",
                    IsActive = true,
                    IsEmailVerified = false
                };
                await _uow.Users.AddAsync(newUser);
                await _uow.SaveChangesAsync();
                userId = newUser.Id;
            }
        }
        if (!userId.HasValue || userId.Value <= 0) return null;

        // Idempotent short-circuit
        var existingSession = await _uow.CheckoutSessions.GetByUserAndKeyAsync(userId.Value, idempotencyKey);
        if (existingSession?.OrderId is int existingOrderId)
        {
            var existingOrder = await _uow.Orders.GetOrderWithItemsAsync(existingOrderId);
            if (existingOrder != null) return _mapper.Map<OrderDto>(existingOrder);
        }

        await _uow.BeginTransactionAsync();
        try
        {
            CartEntity? cart = null;
            if (userId.HasValue && userId.Value > 0)
            {
                cart = await _uow.Carts.GetByUserIdAsync(userId.Value);
            }
            if (cart == null && !string.IsNullOrWhiteSpace(sessionId))
            {
                cart = await _uow.Carts.GetBySessionIdAsync(sessionId!);
            }

            if (cart == null || cart.Items.Count == 0)
            {
                await _uow.RollbackTransactionAsync();
                return null;
            }

            cart = await _uow.Carts.GetCartWithItemsAsync(cart.Id) ?? cart;
            if (cart.Items.Count == 0)
            {
                await _uow.RollbackTransactionAsync();
                return null;
            }

            var cartItems = cart.Items.ToList();
            var now = DateTime.UtcNow;
            var reservations = new List<StockReservation>();

            var totalWeight = cartItems.Sum(i => i.Quantity * (i.ProductVariant?.Weight ?? i.Product.Weight ?? 0m));

            foreach (var item in cartItems)
            {
                var variant = item.ProductVariantId.HasValue ? item.ProductVariant : null;
                var reservedQty = await _uow.StockReservations.GetActiveReservedQuantityAsync(item.ProductId, item.ProductVariantId);
                var available = (variant?.Stock ?? item.Product.Stock) - reservedQty;
                if (item.Quantity > available)
                {
                    await _uow.RollbackTransactionAsync();
                    return null;
                }

                reservations.Add(new StockReservation
                {
                    ProductId = item.ProductId,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    UserId = userId.Value,
                    IdempotencyKey = idempotencyKey,
                    ExpiresAt = now.AddMinutes(15),
                    Status = "Reserved"
                });
            }

            var subTotal = cartItems.Sum(i => i.Quantity * i.UnitPrice);

            // Shipping cost: prefer provided value, else compute from weight/region
            var shipping = request.Checkout.ShippingCost;
            var shippingWeight = totalWeight > 0 ? totalWeight : request.Checkout.TotalWeight;
            if (shipping <= 0 && shippingWeight > 0 && !string.IsNullOrWhiteSpace(shippingRegion))
            {
                var rate = await _uow.ShippingRates.GetRateAsync(shippingRegion!, shippingWeight);
                if (rate != null)
                {
                    shipping = rate.Price;
                }
            }

            var taxRegion = string.IsNullOrWhiteSpace(shippingRegion) ? null : shippingRegion;
            var tax = 0m;
            if (taxRegion != null)
            {
                var taxRate = await _uow.TaxRates.GetByRegionAsync(taxRegion);
                if (taxRate != null && taxRate.RatePercent > 0)
                {
                    var taxableAmount = subTotal + shipping;
                    if (taxableAmount > 0)
                    {
                        tax = Math.Round(taxableAmount * (taxRate.RatePercent / 100m), 2, MidpointRounding.AwayFromZero);
                    }
                }
            }

            var discount = 0m;
            int? couponId = null;
            string? couponCode = null;
            CouponRedemption? redemption = null;

            if (!string.IsNullOrWhiteSpace(request.Checkout.CouponCode))
            {
                var coupon = await _uow.Coupons.GetByCodeAsync(request.Checkout.CouponCode.Trim());
                if (coupon != null && coupon.IsActive)
                {
                    var orderAmountForCoupon = subTotal + shipping + tax;
                    var inWindow = (!coupon.StartsAt.HasValue || coupon.StartsAt.Value <= now) && (!coupon.ExpiresAt.HasValue || coupon.ExpiresAt.Value >= now);
                    var underMax = !coupon.MaxUsageCount.HasValue || coupon.UsageCount < coupon.MaxUsageCount.Value;
                    var meetsMin = !coupon.MinOrderAmount.HasValue || orderAmountForCoupon >= coupon.MinOrderAmount.Value;
                    var alreadyRedeemed = coupon.IsSingleUsePerUser && (await _uow.CouponRedemptions.GetByUserAndCouponAsync(userId.Value, coupon.Id)) != null;

                    if (inWindow && underMax && meetsMin && !alreadyRedeemed)
                    {
                        discount = CalculateDiscount(coupon.DiscountType, coupon.Amount, orderAmountForCoupon);
                        couponId = coupon.Id;
                        couponCode = coupon.Code;
                        coupon.UsageCount += 1;
                        await _uow.Coupons.UpdateAsync(coupon);

                        if (coupon.IsSingleUsePerUser)
                        {
                            redemption = new CouponRedemption
                            {
                                CouponId = coupon.Id,
                                UserId = userId.Value,
                                IdempotencyKey = idempotencyKey
                            };
                            await _uow.CouponRedemptions.AddAsync(redemption);
                        }
                    }
                }
            }

            var orderItems = cartItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductVariantId = i.ProductVariantId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList();

            if (tax > 0 && subTotal > 0 && orderItems.Count > 0)
            {
                decimal allocated = 0;
                for (int idx = 0; idx < orderItems.Count; idx++)
                {
                    var lineTotal = orderItems[idx].TotalPrice;
                    if (idx == orderItems.Count - 1)
                    {
                        orderItems[idx].TaxAmount = tax - allocated;
                    }
                    else
                    {
                        var share = Math.Round(tax * (lineTotal / subTotal), 2, MidpointRounding.AwayFromZero);
                        orderItems[idx].TaxAmount = share;
                        allocated += share;
                    }
                }
            }

            var total = subTotal + shipping + tax - discount;
            if (total < 0) total = 0;

        // Legal field validation (Turkey specific)
        if (string.Equals(shippingRegion, "TR", StringComparison.OrdinalIgnoreCase))
        {
            if (request.Checkout.IsCorporate)
            {
                if (!IdentityValidator.ValidateTaxNumber(request.Checkout.TaxNumber))
                {
                    await _uow.RollbackTransactionAsync();
                    return null;
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.Checkout.TcKimlikNo))
            {
                if (!IdentityValidator.ValidateTcKimlikNo(request.Checkout.TcKimlikNo))
                {
                    await _uow.RollbackTransactionAsync();
                    return null;
                }
            }
        }

            var order = new Order
            {
                UserId = userId.Value,
                OrderNumber = GenerateOrderNumber(),
                SubTotal = subTotal,
                ShippingCost = shipping,
                Tax = tax,
                DiscountAmount = discount,
                TotalAmount = total,
                ShippingRegion = shippingRegion,
                ShippingWeight = shippingWeight,
                TaxRegion = taxRegion,
                CouponId = couponId,
                CouponCode = couponCode,
                IdempotencyKey = idempotencyKey,
                Status = "Pending",
                ShippingAddress = request.Checkout.ShippingAddress,
                BillingAddress = request.Checkout.BillingAddress,
                Notes = request.Checkout.Notes,
            IsCorporate = request.Checkout.IsCorporate,
            TcKimlikNo = request.Checkout.TcKimlikNo,
            TaxNumber = request.Checkout.TaxNumber,
            TaxOffice = request.Checkout.TaxOffice,
            CompanyName = request.Checkout.CompanyName,
            KvkkApproved = request.Checkout.KvkkApproved,
                Items = orderItems
            };

            await _uow.Orders.AddAsync(order);
            foreach (var reservation in reservations)
            {
                await _uow.StockReservations.AddAsync(reservation);
            }

            var checkoutSession = existingSession ?? new CheckoutSession
            {
                UserId = userId.Value,
                IdempotencyKey = idempotencyKey
            };
            if (existingSession == null)
            {
                await _uow.CheckoutSessions.AddAsync(checkoutSession);
            }

            // Clear cart
            cart.Items.Clear();
            await _uow.Carts.UpdateAsync(cart);

            await _uow.SaveChangesAsync();

            // Commit reservations and redemption to the order
            foreach (var reservation in reservations)
            {
                reservation.OrderId = order.Id;
                reservation.Status = "Committed";
                var cartItem = cartItems.FirstOrDefault(i => i.ProductId == reservation.ProductId && i.ProductVariantId == reservation.ProductVariantId);
                if (cartItem?.ProductVariant != null)
                {
                    cartItem.ProductVariant.Stock -= reservation.Quantity;
                    await _uow.ProductVariants.UpdateAsync(cartItem.ProductVariant);
                }
                else if (cartItem?.Product != null)
                {
                    cartItem.Product.Stock -= reservation.Quantity;
                    await _uow.Products.UpdateAsync(cartItem.Product);
                }
            }

            if (redemption != null)
            {
                redemption.OrderId = order.Id;
                await _uow.CouponRedemptions.UpdateAsync(redemption);
            }

            checkoutSession.OrderId = order.Id;
            checkoutSession.TotalAmount = order.TotalAmount;
            await _uow.CheckoutSessions.UpdateAsync(checkoutSession);

            await _uow.SaveChangesAsync();
            await _uow.CommitTransactionAsync();

            _cache.Remove(AllOrdersKey);

            return _mapper.Map<OrderDto>(order);
        }
        catch
        {
            await _uow.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> Handle(UnapplyCouponCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return false;
        if (request.RequesterUserId > 0 && order.UserId != request.RequesterUserId)
            return false;

        var couponId = order.CouponId;
        order.CouponId = null;
        order.CouponCode = null;
        order.DiscountAmount = 0;
        order.TotalAmount = order.SubTotal + order.ShippingCost + order.Tax;

        if (couponId.HasValue)
        {
            var coupon = await _uow.Coupons.GetByIdAsync(couponId.Value);
            if (coupon != null && coupon.UsageCount > 0)
            {
                coupon.UsageCount -= 1;
                await _uow.Coupons.UpdateAsync(coupon);
            }
        }

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{request.OrderId}");
        return true;
    }

    public async Task<bool> Handle(UpdateOrderRefundCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return false;

        var wasRefundRequested = order.RefundRequested;
        var wasRefunded = order.Refunded;

        if (request.UpdateDto.RefundRequested.HasValue)
            order.RefundRequested = request.UpdateDto.RefundRequested.Value;
        if (request.UpdateDto.Refunded.HasValue)
            order.Refunded = request.UpdateDto.Refunded.Value;
        if (request.UpdateDto.AdminNote != null)
            order.AdminNote = request.UpdateDto.AdminNote;

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{request.OrderId}");

        if (order.RefundRequested && !wasRefundRequested)
        {
            var subject = $"İade talebiniz alındı - {order.OrderNumber}";
            var body = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişiniz için iade talebi oluşturuldu.";
            await SendUserEmailAsync(order, subject, body);
        }

        if (order.Refunded && !wasRefunded)
        {
            var subject = $"İadeniz tamamlandı - {order.OrderNumber}";
            var body = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişiniz için iade işlemi tamamlandı.";
            await SendUserEmailAsync(order, subject, body);
        }
        return true;
    }

    public async Task<bool> Handle(UpdateOrderNoteCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return false;
        order.AdminNote = request.AdminNote;
        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{request.OrderId}");
        return true;
    }

    public async Task<bool> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return false;

        if (request.RequesterUserId > 0 && order.UserId != request.RequesterUserId)
            return false;

        var coupon = await _uow.Coupons.GetByCodeAsync(request.ApplyDto.Code.Trim());
        if (coupon == null || !coupon.IsActive) return false;

        var now = DateTime.UtcNow;
        if (coupon.StartsAt.HasValue && coupon.StartsAt.Value > now) return false;
        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < now) return false;
        if (coupon.MaxUsageCount.HasValue && coupon.UsageCount >= coupon.MaxUsageCount.Value) return false;
        var orderAmount = request.ApplyDto.OrderAmount ?? (order.SubTotal + order.ShippingCost + order.Tax);
        if (orderAmount < 0) return false;

        if (coupon.MinOrderAmount.HasValue && orderAmount < coupon.MinOrderAmount.Value) return false;

        var discountAmount = CalculateDiscount(coupon.DiscountType, coupon.Amount, orderAmount);
        order.CouponId = coupon.Id;
        order.CouponCode = coupon.Code;
        order.DiscountAmount = discountAmount;
        order.TotalAmount = order.SubTotal + order.ShippingCost + order.Tax - discountAmount;

        await _uow.Orders.UpdateAsync(order);

        coupon.UsageCount += 1;
        await _uow.Coupons.UpdateAsync(coupon);

        await _uow.SaveChangesAsync();
        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{request.OrderId}");
        return true;
    }

    private static decimal CalculateDiscount(string discountType, decimal amount, decimal orderAmount)
    {
        var normalizedType = discountType?.Trim().ToLowerInvariant();
        decimal discount = normalizedType == "fixed" ? amount : orderAmount * (amount / 100m);
        if (discount < 0) return 0;
        return discount > orderAmount ? orderAmount : discount;
    }

    private async Task SendUserEmailAsync(Order order, string subject, string body)
    {
        try
        {
            var email = order.User?.Email;
            if (string.IsNullOrWhiteSpace(email))
            {
                var user = await _uow.Users.GetByIdAsync(order.UserId);
                email = user?.Email;
            }

            if (string.IsNullOrWhiteSpace(email)) return;

            await _emailService.SendAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order email for {OrderNumber}", order.OrderNumber);
        }
    }

    private static string MapPaymentStatus(string providerStatus)
    {
        if (string.IsNullOrWhiteSpace(providerStatus)) return "Pending";

        return providerStatus.ToLowerInvariant() switch
        {
            "succeeded" => "Succeeded",
            "requires_action" => "RequiresAction",
            "requires_payment_method" => "Failed",
            _ => "Pending"
        };
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Random.Shared.Next(100, 999)}";
    }

    public async Task<PaymentIntentDto?> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return null;

        if (request.RequesterUserId.HasValue && request.RequesterUserId.Value > 0)
        {
            if (order.UserId != request.RequesterUserId.Value) return null;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.IdempotencyKey)) return null;
            if (!string.Equals(order.IdempotencyKey, request.IdempotencyKey, StringComparison.Ordinal)) return null;
        }

        if (string.Equals(order.PaymentStatus, "Failed-Permanent", StringComparison.OrdinalIgnoreCase))
            return null;

        var amount = order.TotalAmount;
        if (amount <= 0) return null;

        var session = await _uow.CheckoutSessions.GetByOrderIdAsync(order.Id) ??
                      (string.IsNullOrWhiteSpace(order.IdempotencyKey)
                        ? null
                        : await _uow.CheckoutSessions.GetByUserAndKeyAsync(order.UserId, order.IdempotencyKey));

        if (session == null && !string.IsNullOrWhiteSpace(order.IdempotencyKey))
        {
            session = new CheckoutSession
            {
                UserId = order.UserId,
                IdempotencyKey = order.IdempotencyKey,
                OrderId = order.Id,
                TotalAmount = order.TotalAmount
            };
            await _uow.CheckoutSessions.AddAsync(session);
            await _uow.SaveChangesAsync();
        }

        if (session?.PaymentIntentId is string existingIntent)
        {
            order.PaymentIntentId = existingIntent;
        }

        var (paymentIntentId, clientSecret, status) = await _paymentService.CreateOrUpdatePaymentIntentAsync(order, "usd");

        order.PaymentIntentId = paymentIntentId;
        order.PaymentStatus = MapPaymentStatus(status);
        order.PaymentProvider = "Stripe";
        if (session != null)
        {
            session.PaymentIntentId = paymentIntentId;
            session.TotalAmount = order.TotalAmount;
            await _uow.CheckoutSessions.UpdateAsync(session);
        }

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        return new PaymentIntentDto
        {
            PaymentIntentId = paymentIntentId,
            ClientSecret = clientSecret,
            Provider = "Stripe",
            Amount = amount,
            Currency = "usd"
        };
    }

    public async Task<bool> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByPaymentIntentIdAsync(request.PaymentIntentId);
        if (order == null) return false;

        order.PaymentStatus = request.Status;
        order.PaymentProvider = request.Provider;

        const int maxRetries = 3;

        if (string.Equals(request.Status, "Failed", StringComparison.OrdinalIgnoreCase))
        {
            order.PaymentRetryCount += 1;
            if (order.PaymentRetryCount >= maxRetries)
            {
                order.PaymentStatus = "Failed-Permanent";
            }
        }
        else if (string.Equals(request.Status, "Succeeded", StringComparison.OrdinalIgnoreCase))
        {
            order.PaymentRetryCount = 0;
        }

        if (string.Equals(request.Status, "Refunded", StringComparison.OrdinalIgnoreCase))
        {
            order.Refunded = true;
            order.RefundRequested = false;
            order.Status = "Cancelled";
        }
        else if (string.Equals(request.Status, "Succeeded", StringComparison.OrdinalIgnoreCase))
        {
            // Eğer iş akışında gerekli ise sipariş durumunu da ilerletebiliriz
            if (order.Status == "Pending")
            {
                order.Status = "Processing";
            }

            if (string.IsNullOrWhiteSpace(order.InvoiceUrl))
            {
                try
                {
                    order.InvoiceUrl = await _invoiceService.GenerateInvoiceAsync(order, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Invoice generation failed for order {OrderNumber}", order.OrderNumber);
                }
            }

            // E-posta bildirimi (best-effort)
            if (!string.IsNullOrEmpty(order.User?.Email))
            {
                var subject = $"Ödemeniz alındı - Sipariş {order.OrderNumber}";
                var body = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişinizin ödemesi başarıyla alındı. Toplam: {order.TotalAmount:C}.";
                if (!string.IsNullOrWhiteSpace(order.InvoiceUrl))
                {
                    body += $"\nFaturanızı buradan görüntüleyebilirsiniz: {order.InvoiceUrl}";
                }
                body += "\nTeşekkürler.";
                try { await _emailService.SendAsync(order.User.Email, subject, body); } catch (Exception ex) { _logger.LogError(ex, "Failed to send payment receipt email for order {OrderNumber}", order.OrderNumber); }
            }
        }
        else if (string.Equals(request.Status, "Failed", StringComparison.OrdinalIgnoreCase))
        {
            // Ödeme başarısızsa sipariş durumunu değiştirmek opsiyonel; şimdilik dokunmuyoruz
        }

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        if (string.Equals(request.Status, "Refunded", StringComparison.OrdinalIgnoreCase))
        {
            var subject = $"İadeniz tamamlandı - {order.OrderNumber}";
            var body = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişiniz için iade işlemi tamamlandı.";
            await SendUserEmailAsync(order, subject, body);
        }

        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{order.Id}");
        return true;
    }

    public async Task<bool> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null || string.IsNullOrWhiteSpace(order.PaymentIntentId)) return false;

        var amountToRefund = request.Amount ?? order.TotalAmount;
        if (amountToRefund <= 0) return false;

        var status = await _paymentService.RefundPaymentAsync(order.PaymentIntentId!, amountToRefund);

        order.PaymentStatus = status;
        order.Refunded = string.Equals(status, "succeeded", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(status, "refunded", StringComparison.OrdinalIgnoreCase);
        order.RefundAmount = Math.Min(order.TotalAmount, order.RefundAmount + amountToRefund);
        order.RefundRequested = false;

        if (order.Refunded && amountToRefund >= order.TotalAmount)
        {
            order.Status = "Cancelled";
        }

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        var refundSubject = $"İade işlendi - {order.OrderNumber}";
        var refundBody = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişiniz için {amountToRefund:C} tutarında iade başlatıldı. Sağlayıcı durumu: {status}.";
        await SendUserEmailAsync(order, refundSubject, refundBody);

        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{order.Id}");
        return true;
    }

    public async Task<bool> Handle(UpdateOrderTrackingCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return false;

        var oldTracking = order.TrackingNumber;
        order.TrackingNumber = request.Tracking.TrackingNumber?.Trim();
        if (!string.IsNullOrWhiteSpace(request.Tracking.Status))
        {
            order.Status = request.Tracking.Status;
        }

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{request.OrderId}");

        if (!string.IsNullOrWhiteSpace(order.TrackingNumber) && order.TrackingNumber != oldTracking)
        {
            var subject = $"Siparişiniz kargoya verildi - {order.OrderNumber}";
            var body = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişiniz kargoya verilmiştir.\nTakip Numarası: {order.TrackingNumber}";
            await SendUserEmailAsync(order, subject, body);
        }
        return true;
    }

    public async Task<bool> Handle(RequestRefundCommand request, CancellationToken cancellationToken)
    {
        var order = await _uow.Orders.GetByIdAsync(request.OrderId);
        if (order == null || order.UserId != request.UserId) return false;

        order.RefundRequested = true;
        if (!string.IsNullOrWhiteSpace(request.Request.Reason))
        {
            order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                ? request.Request.Reason
                : $"{order.Notes}\nRefund Reason: {request.Request.Reason}";
        }

        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();

        _cache.Remove(AllOrdersKey);
        _cache.Remove($"order_{order.Id}");

        var subject = $"İade talebiniz alındı - {order.OrderNumber}";
        var body = $"Merhaba,\n\n{order.OrderNumber} numaralı siparişiniz için iade talebinizi aldık ve incelemeye başladık.";
        await SendUserEmailAsync(order, subject, body);
        return true;
    }
}
