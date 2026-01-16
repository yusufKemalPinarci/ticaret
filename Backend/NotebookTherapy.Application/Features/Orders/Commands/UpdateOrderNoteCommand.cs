using MediatR;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record UpdateOrderNoteCommand(int OrderId, string AdminNote) : IRequest<bool>;
