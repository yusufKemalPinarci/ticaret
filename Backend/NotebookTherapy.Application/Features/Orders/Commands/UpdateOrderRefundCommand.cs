using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record UpdateOrderRefundCommand(int OrderId, UpdateOrderRefundDto UpdateDto) : IRequest<bool>;
