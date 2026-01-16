using MediatR;

namespace NotebookTherapy.Application.Features.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<bool>;
