using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Intuix.Authentication.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Intuix.Authentication.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;
    public AuthDbContext(DbContextOptions<AuthDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        //modelBuilder.ApplyConfiguration(new TenantConfiguration());
        //modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
        //modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        //modelBuilder.ApplyConfiguration(new RoleConfiguration());
        //modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        //modelBuilder.ApplyConfiguration(new UserConfiguration());
        //modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        //modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        //modelBuilder.ApplyConfiguration(new UserCompanyConfiguration());
        //modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

        modelBuilder.Entity<User>()
        .HasQueryFilter(u => u.TenantId == _currentUser.TenantId);

        modelBuilder.Entity<Role>()
            .HasQueryFilter(r => r.TenantId == _currentUser.TenantId);

        modelBuilder.Entity<Organization>()
            .HasQueryFilter(o => o.TenantId == _currentUser.TenantId);


        base.OnModelCreating(modelBuilder);
    }
}
