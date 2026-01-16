using MediatR;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record UpdateOrderStatusCommand(int OrderId, string Status) : IRequest<bool>;
