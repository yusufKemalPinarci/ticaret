using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Products;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
