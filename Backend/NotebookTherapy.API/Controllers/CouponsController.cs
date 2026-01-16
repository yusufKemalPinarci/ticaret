using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Coupons;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CouponsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("{code}/validate")]
    public async Task<ActionResult<CouponValidationResultDto>> Validate(string code, [FromQuery] decimal orderAmount)
    {
        if (orderAmount < 0)
        {
            return BadRequest(new CouponValidationResultDto { IsValid = false, Message = "Order amount must be non-negative." });
        }

        var result = await _mediator.Send(new ValidateCouponQuery(code, orderAmount));
        if (!result.IsValid)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
