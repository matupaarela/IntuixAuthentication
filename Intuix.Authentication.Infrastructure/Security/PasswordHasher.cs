using Intuix.Authentication.Domain.Interfaces;
using System.Security.Cryptography;

namespace Intuix.Authentication.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public byte[] Hash(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            HashAlgorithmName.SHA256);

        var salt = algorithm.Salt;
        var key = algorithm.GetBytes(KeySize);

        return salt.Concat(key).ToArray();
    }

    public bool Verify(string password, byte[] hash)
    {
        var salt = hash.Take(SaltSize).ToArray();
        var storedKey = hash.Skip(SaltSize).ToArray();

        using var algorithm = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        var key = algorithm.GetBytes(KeySize);

        return key.SequenceEqual(storedKey);
    }
}