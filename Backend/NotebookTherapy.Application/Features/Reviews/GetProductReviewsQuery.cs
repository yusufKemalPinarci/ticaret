using MediatR;
using NotebookTherapy.Application.DTOs;
using System.Collections.Generic;

namespace NotebookTherapy.Application.Features.Reviews;

public record GetProductReviewsQuery(int ProductId) : IRequest<IEnumerable<ReviewDto>>;
