using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Orders.Commands;
using NotebookTherapy.Application.Features.Orders;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout([FromBody] CheckoutDto dto)
    {
        var userId = GetUserId();
        var sessionId = dto.SessionId;
        if (string.IsNullOrWhiteSpace(dto.IdempotencyKey)) return BadRequest("Idempotency key gerekli.");
        if (!string.Equals(dto.ShippingRegion, "TR", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Bu bölge yakında desteklenecek.");
        if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("E-posta gerekli.");

        var order = await _mediator.Send(new CreateOrderFromCartCommand(userId, sessionId, dto));
        if (order == null) return BadRequest("Sepet boş veya sipariş oluşturulamadı.");
        return Ok(order);
    }

    [Authorize]
    [HttpPut("{id}/coupon")]
    public async Task<ActionResult> ApplyCoupon(int id, [FromBody] ApplyCouponDto dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var result = await _mediator.Send(new ApplyCouponCommand(id, userId.Value, dto));
        if (!result) return BadRequest();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}/coupon")]
    public async Task<ActionResult> UnapplyCoupon(int id)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var result = await _mediator.Send(new UnapplyCouponCommand(id, userId.Value));
        if (!result) return NotFound();
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/refund-request")]
    public async Task<ActionResult> RequestRefund(int id, [FromBody] RefundRequestDto dto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var ok = await _mediator.Send(new RequestRefundCommand(id, userId.Value, dto));
        if (!ok) return NotFound();
        return Accepted();
    }

    [AllowAnonymous]
    [HttpPost("{id}/payment-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreatePaymentIntent(int id, [FromBody] PaymentIntentRequestDto dto)
    {
        var userId = GetUserId();
        var intent = await _mediator.Send(new CreatePaymentIntentCommand(id, userId, dto.IdempotencyKey));
        if (intent == null) return BadRequest();
        return Ok(intent);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized();

        var orders = await _mediator.Send(new GetMyOrdersQuery(userId.Value));
        return Ok(orders);
    }

    private int? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
    }
}

public record PaymentIntentRequestDto(string? IdempotencyKey, string? SessionId);
