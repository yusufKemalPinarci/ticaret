using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.ShippingRates.Commands;
using NotebookTherapy.Application.Features.ShippingRates.Queries;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/admin/shipping-rates")]
[Authorize(Roles = "Admin")]
public class AdminShippingRatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminShippingRatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShippingRateDto>>> GetAll()
    {
        var rates = await _mediator.Send(new GetAllShippingRatesQuery());
        return Ok(rates);
    }

    [HttpPost]
    public async Task<ActionResult<ShippingRateDto>> Create([FromBody] CreateShippingRateDto dto)
    {
        var created = await _mediator.Send(new CreateShippingRateCommand(dto));
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateShippingRateDto dto)
    {
        var ok = await _mediator.Send(new UpdateShippingRateCommand(id, dto));
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var ok = await _mediator.Send(new DeleteShippingRateCommand(id));
        if (!ok) return NotFound();
        return NoContent();
    }
}
