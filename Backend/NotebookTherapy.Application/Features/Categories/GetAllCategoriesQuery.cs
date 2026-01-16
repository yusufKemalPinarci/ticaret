using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Categories;

public record GetAllCategoriesQuery() : IRequest<IEnumerable<CategoryDto>>;
