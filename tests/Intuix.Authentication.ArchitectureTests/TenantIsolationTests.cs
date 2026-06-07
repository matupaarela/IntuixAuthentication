using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Intuix.Authentication.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Intuix.Authentication.ArchitectureTests;

public class TenantIsolationTests
{
    [Fact]
    public async Task Tenant_context_can_scope_queries_without_authenticated_user()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        var tenantContext = new TestTenantContext();
        using var scope = CreateScope(tenantContext);
        await SeedAsync(scope.Context, tenantA, tenantB);

        tenantContext.SetTenant(tenantA);

        Assert.Equal(1, await scope.Context.Users.CountAsync());
        Assert.Equal(1, await scope.Context.Roles.CountAsync());
        Assert.Equal(1, await scope.Context.RefreshTokens.CountAsync());
    }

    [Theory]
    [MemberData(nameof(FilteredEntityTypes))]
    public void Filtered_entities_define_query_filters(Type entityType)
    {
        using var scope = CreateScope(Guid.NewGuid());

        var entity = scope.Context.Model.FindEntityType(entityType);

        Assert.NotNull(entity);
        Assert.NotNull(entity!.GetQueryFilter());
    }

    [Fact]
    public async Task Tenant_filters_hide_other_tenant_rows()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        using var scope = CreateScope(tenantA);
        await SeedAsync(scope.Context, tenantA, tenantB);

        Assert.Equal(2, await scope.Context.Tenants.CountAsync());
        Assert.Equal(1, await scope.Context.Organizations.CountAsync());
        Assert.Equal(1, await scope.Context.Companies.CountAsync());
        Assert.Equal(1, await scope.Context.Users.CountAsync());
        Assert.Equal(1, await scope.Context.Roles.CountAsync());
        Assert.Equal(1, await scope.Context.UserCompanies.CountAsync());
        Assert.Equal(1, await scope.Context.UserRoles.CountAsync());
        Assert.Equal(1, await scope.Context.RolePermissions.CountAsync());
        Assert.Equal(1, await scope.Context.RefreshTokens.CountAsync());
    }

    public static IEnumerable<object[]> FilteredEntityTypes() =>
        new[]
        {
            new object[] { typeof(User) },
            new object[] { typeof(Role) },
            new object[] { typeof(Organization) },
            new object[] { typeof(Company) },
            new object[] { typeof(UserCompany) },
            new object[] { typeof(UserRole) },
            new object[] { typeof(RolePermission) },
            new object[] { typeof(RefreshToken) }
        };

    private static async Task SeedAsync(AuthDbContext context, Guid tenantA, Guid tenantB)
    {
        var now = DateTime.UtcNow;

        var tenantEntityA = new Tenant
        {
            Id = tenantA,
            Name = "Tenant A",
            Code = "TENANT_A",
            IsActive = true,
            CreatedAt = now
        };

        var tenantEntityB = new Tenant
        {
            Id = tenantB,
            Name = "Tenant B",
            Code = "TENANT_B",
            IsActive = true,
            CreatedAt = now
        };

        var organizationA = new Organization
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Org A",
            IsActive = true,
            CreatedAt = now
        };
        organizationA.Tenant = tenantEntityA;

        var organizationB = new Organization
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Org B",
            IsActive = true,
            CreatedAt = now
        };
        organizationB.Tenant = tenantEntityB;

        var companyA = new Company
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationA.Id,
            Name = "Company A",
            IsActive = true
        };
        companyA.Organization = organizationA;

        var companyB = new Company
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationB.Id,
            Name = "Company B",
            IsActive = true
        };
        companyB.Organization = organizationB;

        var userA = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Username = "user-a",
            Email = "user-a@example.com",
            PasswordHash = "hash-a",
            IsActive = true,
            IsLocked = false,
            FailedAttempts = 0,
            CreatedAt = now
        };

        var userB = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Username = "user-b",
            Email = "user-b@example.com",
            PasswordHash = "hash-b",
            IsActive = true,
            IsLocked = false,
            FailedAttempts = 0,
            CreatedAt = now
        };

        var roleA = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantA,
            Name = "Role A"
        };

        var roleB = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            Name = "Role B"
        };

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = "TEST_PERMISSION",
            Description = "Test permission"
        };

        var userCompanyA = new UserCompany
        {
            UserId = userA.Id,
            CompanyId = companyA.Id,
            IsDefault = true
        };
        userCompanyA.User = userA;
        userCompanyA.Company = companyA;

        var userCompanyB = new UserCompany
        {
            UserId = userB.Id,
            CompanyId = companyB.Id,
            IsDefault = true
        };
        userCompanyB.User = userB;
        userCompanyB.Company = companyB;

        var userRoleA = new UserRole
        {
            UserId = userA.Id,
            RoleId = roleA.Id
        };
        userRoleA.User = userA;
        userRoleA.Role = roleA;

        var userRoleB = new UserRole
        {
            UserId = userB.Id,
            RoleId = roleB.Id
        };
        userRoleB.User = userB;
        userRoleB.Role = roleB;

        var rolePermissionA = new RolePermission
        {
            RoleId = roleA.Id,
            PermissionId = permission.Id
        };
        rolePermissionA.Role = roleA;
        rolePermissionA.Permission = permission;

        var rolePermissionB = new RolePermission
        {
            RoleId = roleB.Id,
            PermissionId = permission.Id
        };
        rolePermissionB.Role = roleB;
        rolePermissionB.Permission = permission;

        var refreshTokenA = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userA.Id,
            TokenHash = new byte[] { 1, 2, 3 },
            ExpiresAt = now.AddDays(7),
            CreatedAt = now
        };
        refreshTokenA.User = userA;

        var refreshTokenB = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userB.Id,
            TokenHash = new byte[] { 4, 5, 6 },
            ExpiresAt = now.AddDays(7),
            CreatedAt = now
        };
        refreshTokenB.User = userB;

        context.AddRange(tenantEntityA, tenantEntityB);
        await context.SaveChangesAsync();

        context.AddRange(organizationA, organizationB, permission);
        await context.SaveChangesAsync();

        context.AddRange(companyA, companyB, userA, userB, roleA, roleB);
        await context.SaveChangesAsync();

        context.AddRange(
            userCompanyA,
            userCompanyB,
            userRoleA,
            userRoleB,
            rolePermissionA,
            rolePermissionB,
            refreshTokenA,
            refreshTokenB);

        await context.SaveChangesAsync();
    }

    private static Scope CreateScope(Guid tenantId)
        => CreateScope(new TestCurrentUser(tenantId));

    private static Scope CreateScope(ITenantContext tenantContext)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AuthDbContext(options, tenantContext);
        context.Database.EnsureCreated();

        return new Scope(context, connection);
    }

    private sealed record Scope(AuthDbContext Context, SqliteConnection Connection) : IDisposable
    {
        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }
    }

    private sealed class TestCurrentUser : ICurrentUser, ITenantContext
    {
        public TestCurrentUser(Guid tenantId)
        {
            TenantId = tenantId;
        }

        public Guid UserId { get; private set; } = Guid.NewGuid();
        public Guid TenantId { get; private set; }
        public Guid CompanyId { get; private set; } = Guid.NewGuid();
        public Guid RefreshTokenId { get; private set; } = Guid.NewGuid();
        public bool IsAuthenticated { get; private set; } = true;

        public void SetTenant(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }

    private sealed class TestTenantContext : ITenantContext
    {
        public Guid TenantId { get; private set; }

        public void SetTenant(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }
}
