using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Users;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserDto>>;
