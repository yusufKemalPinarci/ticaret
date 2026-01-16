using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Orders;

public record GetAllOrdersQuery() : IRequest<IEnumerable<OrderDto>>;
