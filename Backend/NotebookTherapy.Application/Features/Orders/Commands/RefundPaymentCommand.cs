using MediatR;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record RefundPaymentCommand(int OrderId, decimal? Amount = null) : IRequest<bool>;
