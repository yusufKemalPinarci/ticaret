using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.TaxRates.Commands;
using NotebookTherapy.Application.Features.TaxRates.Queries;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/admin/tax-rates")]
[Authorize(Roles = "Admin")]
public class AdminTaxRatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminTaxRatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaxRateDto>>> GetAll()
    {
        var rates = await _mediator.Send(new GetAllTaxRatesQuery());
        return Ok(rates);
    }

    [HttpPost]
    public async Task<ActionResult<TaxRateDto>> Create([FromBody] CreateTaxRateDto dto)
    {
        var created = await _mediator.Send(new CreateTaxRateCommand(dto));
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTaxRateDto dto)
    {
        var ok = await _mediator.Send(new UpdateTaxRateCommand(id, dto));
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var ok = await _mediator.Send(new DeleteTaxRateCommand(id));
        if (!ok) return NotFound();
        return NoContent();
    }
}
