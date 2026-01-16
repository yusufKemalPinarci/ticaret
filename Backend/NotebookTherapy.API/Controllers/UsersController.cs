using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotebookTherapy.Application.DTOs;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public ActionResult<AuthResponseDto> Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        var firstName = User.FindFirst("firstName")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value;
        var lastName = User.FindFirst("lastName")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                   ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

        return Ok(new AuthResponseDto
        {
            Token = string.Empty,
            Email = email ?? string.Empty,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            Role = role ?? string.Empty
        });
    }
}
