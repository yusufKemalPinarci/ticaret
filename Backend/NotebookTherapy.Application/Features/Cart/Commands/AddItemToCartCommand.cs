using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Cart.Commands;

public record AddItemToCartCommand(int? UserId, string? SessionId, int ProductId, int? ProductVariantId, int Quantity) : IRequest<CartDto>;
