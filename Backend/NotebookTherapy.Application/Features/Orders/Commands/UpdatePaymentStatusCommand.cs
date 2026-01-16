using MediatR;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record UpdatePaymentStatusCommand(string PaymentIntentId, string Status, string Provider) : IRequest<bool>;
