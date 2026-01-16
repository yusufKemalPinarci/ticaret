using MediatR;

namespace NotebookTherapy.Application.Features.Cart.Commands;

public record ClearCartCommand(int? UserId, string? SessionId) : IRequest<bool>;
