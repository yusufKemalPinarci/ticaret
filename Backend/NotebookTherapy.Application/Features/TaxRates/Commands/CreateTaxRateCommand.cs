using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.TaxRates.Commands;

public record CreateTaxRateCommand(CreateTaxRateDto Rate) : IRequest<TaxRateDto>;
