using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Orders;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDto?>;
