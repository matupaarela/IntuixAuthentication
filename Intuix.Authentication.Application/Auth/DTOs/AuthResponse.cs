namespace Intuix.Authentication.Application.Auth.DTOs;

public class AuthResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
}