using AutoMapper;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Reviews.Handlers;

public class ReviewHandlers :
    IRequestHandler<GetProductReviewsQuery, IEnumerable<ReviewDto>>,
    IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ReviewHandlers(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _uow.Reviews.GetByProductIdAsync(request.ProductId);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = new Review
        {
            ProductId = request.Dto.ProductId,
            UserId = request.UserId,
            Rating = request.Dto.Rating,
            Comment = request.Dto.Comment,
            IsApproved = true // Auto-approve
        };

        await _uow.Reviews.AddAsync(review);
        await _uow.SaveChangesAsync();

        // Reload to get User navigation property for mapping
        var finalReview = await _uow.Reviews.GetByIdWithUserAsync(review.Id);

        return _mapper.Map<ReviewDto>(finalReview);
    }
}
