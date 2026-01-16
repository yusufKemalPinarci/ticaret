using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Categories;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;
