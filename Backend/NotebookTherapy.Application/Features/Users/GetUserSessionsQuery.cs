using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Users;

public record GetUserSessionsQuery(int UserId) : IRequest<IEnumerable<SessionDto>>;
