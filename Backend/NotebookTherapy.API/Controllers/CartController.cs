using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using MediatR;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart([FromQuery] string? sessionId)
    {
        var userId = GetUserId();
        var cart = await _mediator.Send(new NotebookTherapy.Application.Features.Cart.GetCartQuery(userId, sessionId));
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItemToCart([FromBody] AddCartItemRequest request, [FromQuery] string? sessionId)
    {
        var userId = GetUserId();
        var cart = await _mediator.Send(new NotebookTherapy.Application.Features.Cart.Commands.AddItemToCartCommand(userId, sessionId, request.ProductId, request.ProductVariantId, request.Quantity));
        return Ok(cart);
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<ActionResult> RemoveCartItem(int cartItemId)
    {
        var result = await _mediator.Send(new NotebookTherapy.Application.Features.Cart.Commands.RemoveCartItemCommand(cartItemId));
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request)
    {
        var cart = await _mediator.Send(new NotebookTherapy.Application.Features.Cart.Commands.UpdateCartItemCommand(cartItemId, request.Quantity));
        return Ok(cart);
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCart([FromQuery] string? sessionId)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new NotebookTherapy.Application.Features.Cart.Commands.ClearCartCommand(userId, sessionId));
        if (!result)
            return NotFound();
        
        return NoContent();
    }

    [Authorize]
    [HttpPost("merge")]
    public async Task<ActionResult<CartDto>> MergeCart([FromQuery] string sessionId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();
        var cart = await _mediator.Send(new NotebookTherapy.Application.Features.Cart.Commands.MergeCartCommand(userId.Value, sessionId));
        return Ok(cart);
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
    }
}

public class AddCartItemRequest
{
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}
