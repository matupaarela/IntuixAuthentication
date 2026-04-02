namespace Intuix.Authentication.Domain.Entities;

public class Company
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = default!;
    public string? Ruc { get; set; }
    public bool IsActive { get; set; }

    public Organization Organization { get; set; } = default!;
}