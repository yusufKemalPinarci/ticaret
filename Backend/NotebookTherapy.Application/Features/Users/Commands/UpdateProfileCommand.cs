using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Users.Commands;

public record UpdateProfileCommand(int UserId, UpdateProfileDto Dto) : IRequest<bool>;

public record UpdateProfileDto(string FirstName, string LastName, string? PhoneNumber);
