using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.ShippingRates.Queries;

public record GetAllShippingRatesQuery() : IRequest<IEnumerable<ShippingRateDto>>;
