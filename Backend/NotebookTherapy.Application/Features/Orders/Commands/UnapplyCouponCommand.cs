using MediatR;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record UnapplyCouponCommand(int OrderId, int RequesterUserId) : IRequest<bool>;