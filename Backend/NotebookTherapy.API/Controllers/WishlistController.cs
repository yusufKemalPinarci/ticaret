using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Wishlist;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishlistDto>>> Get()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var items = await _mediator.Send(new GetMyWishlistQuery(userId));
        return Ok(items);
    }

    [HttpPost("{productId}")]
    public async Task<ActionResult> Add(int productId)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        await _mediator.Send(new AddToWishlistCommand(userId, productId));
        return NoContent();
    }

    [HttpDelete("{productId}")]
    public async Task<ActionResult> Remove(int productId)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        await _mediator.Send(new RemoveFromWishlistCommand(userId, productId));
        return NoContent();
    }
}
