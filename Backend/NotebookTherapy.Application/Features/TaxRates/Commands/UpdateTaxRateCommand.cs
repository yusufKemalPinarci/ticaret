using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.TaxRates.Commands;

public record UpdateTaxRateCommand(int Id, UpdateTaxRateDto Rate) : IRequest<bool>;
