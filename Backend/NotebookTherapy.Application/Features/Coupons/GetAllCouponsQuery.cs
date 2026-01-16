using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Coupons;

public record GetAllCouponsQuery() : IRequest<IEnumerable<CouponDto>>;
