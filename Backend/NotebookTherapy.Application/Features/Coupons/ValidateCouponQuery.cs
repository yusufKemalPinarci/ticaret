using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Coupons;

public record ValidateCouponQuery(string Code, decimal OrderAmount) : IRequest<CouponValidationResultDto>;
