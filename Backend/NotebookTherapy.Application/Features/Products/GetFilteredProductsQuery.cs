using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Models;

namespace NotebookTherapy.Application.Features.Products;

public record GetFilteredProductsQuery(ProductFilterOptions Options) : IRequest<PagedResultDto<ProductDto>>;
