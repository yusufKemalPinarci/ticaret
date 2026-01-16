using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Cart.Commands;

public record UpdateCartItemCommand(int CartItemId, int Quantity) : IRequest<CartDto>;
