using MediatR;

namespace NotebookTherapy.Application.Features.ShippingRates.Commands;

public record DeleteShippingRateCommand(int Id) : IRequest<bool>;
