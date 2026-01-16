using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Coupons.Commands;

public record CreateCouponCommand(CreateCouponDto CreateDto) : IRequest<CouponDto>;
