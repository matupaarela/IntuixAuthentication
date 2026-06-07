using Intuix.Authentication.Application.Devices.Commands;
using Intuix.Authentication.Application.Devices.DTOs;
using Intuix.Authentication.Application.Devices.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intuix.Authentication.Api.Controllers;

[Route("api/devices")]
[ApiController]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DevicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetSessions()
    {
        var sessions = await _mediator.Send(new DeviceGetListQuery());
        return Ok(new { sessions });
    }

    [HttpDelete("{tokenId:guid}")]
    public async Task<IActionResult> RevokeSession(Guid tokenId)
    {
        await _mediator.Send(new DeviceRevokeSessionCommand(tokenId));
        return Ok(new { message = "Session revoked" });
    }

    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        await _mediator.Send(new DeviceRevokeAllSessionsCommand());
        return Ok(new { message = "All other sessions revoked" });
    }
}
