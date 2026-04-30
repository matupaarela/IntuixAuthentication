namespace Intuix.Authentication.Application.Auth.DTOs;

public class LoginRequest
{
    public string TenantCode { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
