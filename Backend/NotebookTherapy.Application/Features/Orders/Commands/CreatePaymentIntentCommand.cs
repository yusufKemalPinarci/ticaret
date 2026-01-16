using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Orders.Commands;

public record CreatePaymentIntentCommand(int OrderId, int? RequesterUserId, string? IdempotencyKey) : IRequest<PaymentIntentDto?>;
