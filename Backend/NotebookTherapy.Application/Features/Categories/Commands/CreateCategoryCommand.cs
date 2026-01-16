using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Categories.Commands;

public record CreateCategoryCommand(CreateCategoryDto CreateDto) : IRequest<CategoryDto>;
