using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intuix.Authentication.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    private readonly ITenantContext _tenantContext;

    public UserRepository(AuthDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

        return user;
    }

    public async Task<Guid?> GetDefaultCompanyAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await (
            from userCompany in _context.UserCompanies.AsNoTracking()
            join company in _context.Companies.AsNoTracking() on userCompany.CompanyId equals company.Id
            join organization in _context.Organizations.AsNoTracking() on company.OrganizationId equals organization.Id
            where userCompany.UserId == userId
                && userCompany.IsDefault
                && company.IsActive
                && organization.IsActive
                && organization.TenantId == _tenantContext.TenantId
            select (Guid?)company.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where ur.UserId == userId
            select p.Code
        )
        .AsNoTracking()
        .Distinct()
        .ToListAsync(cancellationToken);
    }

    public async Task<List<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await(
           from ur in _context.UserRoles
           join r in _context.Roles on ur.RoleId equals r.Id
           where ur.UserId == userId
           select r.Name
        )
        .AsNoTracking()
        .Distinct()
        .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await (
            from userCompany in _context.UserCompanies.AsNoTracking()
            join company in _context.Companies.AsNoTracking() on userCompany.CompanyId equals company.Id
            join organization in _context.Organizations.AsNoTracking() on company.OrganizationId equals organization.Id
            where userCompany.UserId == userId
                && company.IsActive
                && organization.IsActive
                && organization.TenantId == _tenantContext.TenantId
            select company.Id)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
