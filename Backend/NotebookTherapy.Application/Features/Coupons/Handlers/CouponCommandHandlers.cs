using AutoMapper;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Coupons.Commands;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Application.Features.Coupons.Handlers;

public class CouponCommandHandlers :
    IRequestHandler<CreateCouponCommand, CouponDto>,
    IRequestHandler<UpdateCouponCommand, CouponDto?>,
    IRequestHandler<DeleteCouponCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CouponCommandHandlers(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CouponDto> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = _mapper.Map<Coupon>(request.CreateDto);
        coupon.Code = coupon.Code.Trim().ToUpperInvariant();
        coupon.DiscountType = NormalizeDiscountType(coupon.DiscountType);
        coupon.UsageCount = 0;

        await _uow.Coupons.AddAsync(coupon);
        await _uow.CommitAsync();
        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task<CouponDto?> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _uow.Coupons.GetByIdAsync(request.Id);
        if (coupon == null) return null;

        _mapper.Map(request.UpdateDto, coupon);
        if (!string.IsNullOrWhiteSpace(coupon.Code))
        {
            coupon.Code = coupon.Code.Trim().ToUpperInvariant();
        }
        if (!string.IsNullOrWhiteSpace(coupon.DiscountType))
        {
            coupon.DiscountType = NormalizeDiscountType(coupon.DiscountType);
        }
        coupon.UpdatedAt = DateTime.UtcNow;

        await _uow.Coupons.UpdateAsync(coupon);
        await _uow.CommitAsync();
        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task<bool> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _uow.Coupons.GetByIdAsync(request.Id);
        if (coupon == null) return false;

        await _uow.Coupons.DeleteAsync(coupon);
        await _uow.CommitAsync();
        return true;
    }

    private static string NormalizeDiscountType(string discountType)
    {
        return string.IsNullOrWhiteSpace(discountType)
            ? "Percent"
            : discountType.Trim();
    }
}
