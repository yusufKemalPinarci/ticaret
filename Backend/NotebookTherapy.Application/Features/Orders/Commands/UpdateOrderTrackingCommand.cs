using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record UpdateOrderTrackingCommand(int OrderId, UpdateOrderTrackingDto Tracking) : IRequest<bool>;