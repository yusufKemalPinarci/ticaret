using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Coupons;
using NotebookTherapy.Application.Features.Coupons.Commands;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/admin/coupons")]
[Authorize(Roles = "Admin")]
public class AdminCouponsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCouponsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CouponDto>>> GetAll()
    {
        var coupons = await _mediator.Send(new GetAllCouponsQuery());
        return Ok(coupons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CouponDto>> GetById(int id)
    {
        var coupon = await _mediator.Send(new GetCouponByIdQuery(id));
        if (coupon == null) return NotFound();
        return Ok(coupon);
    }

    [HttpPost]
    public async Task<ActionResult<CouponDto>> Create([FromBody] CreateCouponDto dto)
    {
        var created = await _mediator.Send(new CreateCouponCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CouponDto>> Update(int id, [FromBody] UpdateCouponDto dto)
    {
        var updated = await _mediator.Send(new UpdateCouponCommand(id, dto));
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteCouponCommand(id));
        if (!deleted) return NotFound();
        return NoContent();
    }
}
