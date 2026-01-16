using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Products;

public record GetProductsByCategoryQuery(int CategoryId) : IRequest<IEnumerable<ProductDto>>;
