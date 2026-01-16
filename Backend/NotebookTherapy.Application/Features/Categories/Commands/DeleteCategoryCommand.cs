using MediatR;

namespace NotebookTherapy.Application.Features.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest<bool>;
