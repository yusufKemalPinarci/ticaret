using MediatR;

namespace NotebookTherapy.Application.Features.Users.Commands;

public record UpdateUserRoleCommand(int UserId, string Role) : IRequest<bool>;
