using MediatR;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.Application.Features.Reviews;

public record CreateReviewCommand(int UserId, CreateReviewDto Dto) : IRequest<ReviewDto>;
