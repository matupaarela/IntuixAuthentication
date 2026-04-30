using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intuix.Authentication.Infrastructure.Persistence.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AuthDbContext _context;

        public TenantRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetByCodeAsync(string code)
        {
            return await _context.Tenants.FirstOrDefaultAsync(t => t.Code.ToLower() == code.ToLower());
        }
    }
}
