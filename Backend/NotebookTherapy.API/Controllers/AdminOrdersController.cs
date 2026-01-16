using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using MediatR;
using NotebookTherapy.Application.Features.Orders;
using NotebookTherapy.Application.Features.Orders.Commands;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _mediator.Send(new GetAllOrdersQuery());
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> Get(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var result = await _mediator.Send(new NotebookTherapy.Application.Features.Orders.Commands.UpdateOrderStatusCommand(id, status));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/refund")]
    public async Task<ActionResult> UpdateRefund(int id, [FromBody] UpdateOrderRefundDto dto)
    {
        var result = await _mediator.Send(new UpdateOrderRefundCommand(id, dto));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/note")]
    public async Task<ActionResult> UpdateAdminNote(int id, [FromBody] UpdateOrderNoteDto dto)
    {
        var result = await _mediator.Send(new UpdateOrderNoteCommand(id, dto.AdminNote));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/coupon")]
    public async Task<ActionResult> ApplyCoupon(int id, [FromBody] ApplyCouponDto dto)
    {
        var result = await _mediator.Send(new ApplyCouponCommand(id, 0, dto));
        if (!result) return BadRequest();
        return NoContent();
    }

    [HttpDelete("{id}/coupon")]
    public async Task<ActionResult> UnapplyCoupon(int id)
    {
        var result = await _mediator.Send(new UnapplyCouponCommand(id, 0));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/tracking")]
    public async Task<ActionResult> UpdateTracking(int id, [FromBody] UpdateOrderTrackingDto dto)
    {
        var result = await _mediator.Send(new UpdateOrderTrackingCommand(id, dto));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/refund-payment")]
    public async Task<ActionResult> RefundPayment(int id, [FromBody] RefundPaymentDto dto)
    {
        var amount = dto.Amount;
        var ok = await _mediator.Send(new RefundPaymentCommand(id, amount));
        if (!ok) return BadRequest();
        return NoContent();
    }
}
