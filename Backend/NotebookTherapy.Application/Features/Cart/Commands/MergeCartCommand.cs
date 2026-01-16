using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Cart.Commands;

public record MergeCartCommand(int UserId, string SessionId) : IRequest<CartDto>;
