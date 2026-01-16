using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record CreateOrderFromCartCommand(int? UserId, string? SessionId, CheckoutDto Checkout) : IRequest<OrderDto?>;