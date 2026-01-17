using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Orders;

public record GetMyOrdersQuery(int UserId) : IRequest<IEnumerable<OrderDto>>;
