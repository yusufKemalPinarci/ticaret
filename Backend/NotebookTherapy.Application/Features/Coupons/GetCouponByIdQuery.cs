using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Coupons;

public record GetCouponByIdQuery(int Id) : IRequest<CouponDto?>;
