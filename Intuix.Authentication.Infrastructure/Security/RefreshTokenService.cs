using Intuix.Authentication.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Intuix.Authentication.Infrastructure.Security;
public class RefreshTokenService : IRefreshTokenService
{
    public (string token, byte[] hash) Generate()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomBytes);

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));

        return (token, hash);
    }

    public byte[] Hash(string token)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(token));
    }
}