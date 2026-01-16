using MediatR;

namespace NotebookTherapy.Application.Features.TaxRates.Commands;

public record DeleteTaxRateCommand(int Id) : IRequest<bool>;
