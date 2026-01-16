using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record ApplyCouponCommand(int OrderId, int RequesterUserId, ApplyCouponDto ApplyDto) : IRequest<bool>;
