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

    public Task<User?> GetByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var users = await _context.Users
    .IgnoreQueryFilters()
    .ToListAsync();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        return user;
    }

    public Task<Guid?> GetDefaultCompanyAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetPermissionsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetRolesAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> GetUserCompaniesAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
