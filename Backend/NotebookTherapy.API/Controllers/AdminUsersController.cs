using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MediatR;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Application.Features.Users;
using NotebookTherapy.Application.Features.Users.Commands;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult> UpdateRole(int id, [FromBody] string role)
    {
        var result = await _mediator.Send(new UpdateUserRoleCommand(id, role));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/sessions")]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetSessions(int id)
    {
        var sessions = await _mediator.Send(new GetUserSessionsQuery(id));
        return Ok(sessions);
    }

    [HttpPost("{id}/sessions/revoke")]
    public async Task<ActionResult> RevokeSession(int id, [FromBody] RevokeSessionRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token)) return BadRequest();
        var ok = await _mediator.Send(new RevokeSessionCommand(id, dto.Token, null, true));
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAdmin(int id, [FromBody] UpdateUserAdminDto dto)
    {
        var result = await _mediator.Send(new UpdateUserAdminCommand(id, dto));
        if (!result) return NotFound();
        return NoContent();
    }
}
