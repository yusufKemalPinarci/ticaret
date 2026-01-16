using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Products.Commands;

public record CreateProductCommand(CreateProductDto CreateDto) : IRequest<ProductDto>;
