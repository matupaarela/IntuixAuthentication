namespace Intuix.Authentication.Domain.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public Tenant Tenant { get; set; } = default!;
    public ICollection<Company> Companies { get; set; } = new List<Company>();
}
