using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Products;

public record GetProductsByCollectionQuery(string Collection) : IRequest<IEnumerable<ProductDto>>;
