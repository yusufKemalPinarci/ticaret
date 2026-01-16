using MediatR;

namespace NotebookTherapy.Application.Features.Coupons.Commands;

public record DeleteCouponCommand(int Id) : IRequest<bool>;
