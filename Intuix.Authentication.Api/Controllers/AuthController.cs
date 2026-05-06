using Intuix.Authentication.Application.Auth.Commands.Login;
using Intuix.Authentication.Application.Auth.Commands.RefreshToken;
using Intuix.Authentication.Application.Auth.Commands.SwitchCompany;
using Intuix.Authentication.Application.Auth.DTOs;
using Intuix.Authentication.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intuix.Authentication.Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public AuthController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    // 🔐 LOGIN
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _mediator.Send(
            new LoginCommand(request.Username, request.Password, request.TenantCode));

        return Ok(result);
    }

    // 🔁 REFRESH TOKEN
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request)
    {
        var result = await _mediator.Send(
            new RefreshTokenCommand(request.RefreshToken));

        return Ok(result);
    }

    // 🚪 LOGOUT (revocar token)
    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RefreshTokenRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers["User-Agent"].ToString();
        var device = ParseDevice(userAgent); // opcional

        // lo implementamos en siguiente paso si quieres
        return Ok(new { message = "Token revoked (pending implementation)" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(x => new { x.Type, x.Value });

        return Ok(new
        {
            userId = _currentUser.UserId,
            tenantId = _currentUser.TenantId,
            companyId = _currentUser.CompanyId,
            isAuthenticated = _currentUser.IsAuthenticated,
            claims
        });
    }

    [Authorize]
    [HttpPost("switch-company")]
    public async Task<ActionResult<AuthResponse>> SwitchCompany(SwitchCompanyRequest request)
    {
        var result = await _mediator.Send(
            new SwitchCompanyCommand(request.CompanyId));

        return Ok(result);
    }
}
