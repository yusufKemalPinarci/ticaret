using MediatR;

namespace NotebookTherapy.Application.Features.Cart.Commands;

public record RemoveCartItemCommand(int CartItemId) : IRequest<bool>;
