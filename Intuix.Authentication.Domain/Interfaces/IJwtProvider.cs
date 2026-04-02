using Intuix.Authentication.Domain.Entities;

namespace Intuix.Authentication.Domain.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user, Guid companyId, IEnumerable<string> roles, IEnumerable<string> permissions);
}
