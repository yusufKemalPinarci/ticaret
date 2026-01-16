using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.ShippingRates.Commands;

public record CreateShippingRateCommand(CreateShippingRateDto Rate) : IRequest<ShippingRateDto>;
