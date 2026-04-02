using Intuix.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intuix.Authentication.Infrastructure.Persistence;

public static class AuthDbSeeder
{
    public static async Task SeedAsync(AuthDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var tenantId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        // 🔹 Tenant
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = "Demo Tenant",
            Code = "DEMO"
        };

        // 🔹 Organization
        var org = new Organization
        {
            Id = orgId,
            TenantId = tenantId,
            Name = "Demo Org"
        };

        // 🔹 Company
        var company = new Company
        {
            Id = companyId,
            OrganizationId = orgId,
            Name = "Empresa Demo"
        };

        // 🔹 Password (IMPORTANTE)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");

        // 🔹 User
        var user = new User
        {
            Id = userId,
            TenantId = tenantId,
            Username = "admin",
            Email = "admin@test.com",
            PasswordHash = passwordHash,
            IsActive = true
        };

        // 🔹 Role
        var role = new Role
        {
            Id = roleId,
            TenantId = tenantId,
            Name = "ADMIN"
        };

        // 🔹 Permission
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = "auth.full"
        };

        await context.AddRangeAsync(tenant, org, company, user, role, permission);

        // relaciones
        await context.AddAsync(new UserCompany
        {
            UserId = userId,
            CompanyId = companyId,
            IsDefault = true
        });

        await context.AddAsync(new UserRole
        {
            UserId = userId,
            RoleId = roleId
        });

        await context.AddAsync(new RolePermission
        {
            RoleId = roleId,
            PermissionId = permission.Id
        });

        await context.SaveChangesAsync();
    }
}
