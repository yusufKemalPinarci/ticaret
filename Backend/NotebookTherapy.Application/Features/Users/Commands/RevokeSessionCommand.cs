using MediatR;

namespace NotebookTherapy.Application.Features.Users.Commands;

public record RevokeSessionCommand(int UserId, string Token, string? RevokedByIp, bool IsAdmin) : IRequest<bool>;
