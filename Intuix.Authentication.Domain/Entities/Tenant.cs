namespace Intuix.Authentication.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
}
