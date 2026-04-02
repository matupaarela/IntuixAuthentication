namespace Intuix.Authentication.Domain.Entities;

public class UserCompany
{
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }

    public bool IsDefault { get; set; }

    public User User { get; set; } = default!;
    public Company Company { get; set; } = default!;
}
