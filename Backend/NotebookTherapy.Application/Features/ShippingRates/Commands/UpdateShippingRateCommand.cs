using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.ShippingRates.Commands;

public record UpdateShippingRateCommand(int Id, UpdateShippingRateDto Rate) : IRequest<bool>;
