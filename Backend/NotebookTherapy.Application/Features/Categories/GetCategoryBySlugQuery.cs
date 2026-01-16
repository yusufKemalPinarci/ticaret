using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Categories;

public record GetCategoryBySlugQuery(string Slug) : IRequest<CategoryDto?>;
