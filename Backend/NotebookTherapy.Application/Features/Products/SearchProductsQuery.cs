using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Products;

public record SearchProductsQuery(string SearchTerm) : IRequest<IEnumerable<ProductDto>>;
