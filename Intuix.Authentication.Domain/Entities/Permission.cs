namespace Intuix.Authentication.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
