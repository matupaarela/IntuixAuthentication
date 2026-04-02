namespace Intuix.Authentication.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }

    public int FailedAttempts { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = default!;

    public ICollection<UserCompany> UserCompanies { get; set; } = new List<UserCompany>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
