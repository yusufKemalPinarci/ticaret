using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Products.Commands;

public record UpdateProductCommand(int Id, UpdateProductDto UpdateDto) : IRequest<ProductDto?>;
