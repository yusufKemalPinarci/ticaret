using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.TaxRates.Queries;

public record GetAllTaxRatesQuery() : IRequest<IEnumerable<TaxRateDto>>;
