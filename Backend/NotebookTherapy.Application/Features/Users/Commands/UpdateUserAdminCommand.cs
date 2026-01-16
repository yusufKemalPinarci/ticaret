using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Users.Commands;

public record UpdateUserAdminCommand(int UserId, UpdateUserAdminDto UpdateDto) : IRequest<bool>;
