using AutoMapper;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Application.Features.Coupons;

namespace NotebookTherapy.Application.Features.Coupons.Handlers;

public class CouponQueryHandlers :
    IRequestHandler<GetAllCouponsQuery, IEnumerable<CouponDto>>,
    IRequestHandler<GetCouponByIdQuery, CouponDto?>,
    IRequestHandler<ValidateCouponQuery, CouponValidationResultDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CouponQueryHandlers(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CouponDto>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
    {
        var coupons = await _uow.Coupons.GetAllAsync();
        return _mapper.Map<IEnumerable<CouponDto>>(coupons);
    }

    public async Task<CouponDto?> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
    {
        var coupon = await _uow.Coupons.GetByIdAsync(request.Id);
        if (coupon == null) return null;
        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task<CouponValidationResultDto> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
    {
        var coupon = await _uow.Coupons.GetByCodeAsync(request.Code.Trim());
        if (coupon == null)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon not found." };
        }

        var now = DateTime.UtcNow;
        if (!coupon.IsActive)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon is inactive." };
        }

        if (coupon.StartsAt.HasValue && coupon.StartsAt.Value > now)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon is not active yet." };
        }

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < now)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon has expired." };
        }

        if (coupon.MaxUsageCount.HasValue && coupon.UsageCount >= coupon.MaxUsageCount.Value)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon usage limit reached." };
        }

        if (coupon.MinOrderAmount.HasValue && request.OrderAmount < coupon.MinOrderAmount.Value)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Order total does not meet minimum requirement." };
        }

        var discountAmount = CalculateDiscount(coupon.DiscountType, coupon.Amount, request.OrderAmount);
        return new CouponValidationResultDto
        {
            IsValid = true,
            Message = "Coupon applied.",
            DiscountAmount = discountAmount,
            Coupon = _mapper.Map<CouponDto>(coupon)
        };
    }

    private static decimal CalculateDiscount(string discountType, decimal amount, decimal orderAmount)
    {
        var normalizedType = discountType?.Trim().ToLowerInvariant();
        decimal discount = normalizedType == "fixed" ? amount : orderAmount * (amount / 100m);
        if (discount < 0) return 0;
        return discount > orderAmount ? orderAmount : discount;
    }
}
