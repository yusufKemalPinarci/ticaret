using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Cart;

public record GetCartQuery(int? UserId, string? SessionId) : IRequest<CartDto>;
