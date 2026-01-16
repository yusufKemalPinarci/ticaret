using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Coupons.Commands;

public record UpdateCouponCommand(int Id, UpdateCouponDto UpdateDto) : IRequest<CouponDto?>;
