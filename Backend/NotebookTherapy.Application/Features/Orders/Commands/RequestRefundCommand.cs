using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record RequestRefundCommand(int OrderId, int UserId, RefundRequestDto Request) : IRequest<bool>;
