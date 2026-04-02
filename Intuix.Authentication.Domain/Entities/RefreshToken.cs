namespace Intuix.Authentication.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public byte[] TokenHash { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByToken { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public User User { get; set; } = default!;
}
