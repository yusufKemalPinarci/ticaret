using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Categories.Commands;

public record UpdateCategoryCommand(int Id, UpdateCategoryDto UpdateDto) : IRequest<CategoryDto?>;
