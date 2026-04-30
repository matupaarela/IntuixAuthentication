using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intuix.Authentication.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        return user;
    }

    public async Task<Guid?> GetDefaultCompanyAsync(Guid userId)
    {
        return await _context.UserCompanies
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsDefault)
            .Select(x => (Guid?)x.CompanyId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<string>> GetPermissionsAsync(Guid userId)
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
        .ToListAsync();
    }

    public async Task<List<string>> GetRolesAsync(Guid userId)
    {
        return await(
           from ur in _context.UserRoles
           join r in _context.Roles on ur.RoleId equals r.Id
           where ur.UserId == userId
           select r.Name
       )
       .AsNoTracking()
       .Distinct()
       .ToListAsync();
    }

    public async Task<List<Guid>> GetUserCompaniesAsync(Guid userId)
    {
        return await _context.UserCompanies
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.CompanyId)
            .ToListAsync();
    }
}