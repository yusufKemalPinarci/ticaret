using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Reviews;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByProduct(int productId)
    {
        var reviews = await _mediator.Send(new GetProductReviewsQuery(productId));
        return Ok(reviews);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var review = await _mediator.Send(new CreateReviewCommand(userId, dto));
        return Ok(review);
    }
}
