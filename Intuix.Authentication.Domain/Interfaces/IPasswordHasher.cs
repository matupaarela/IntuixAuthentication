namespace Intuix.Authentication.Domain.Interfaces;

public interface IPasswordHasher
{
    byte[] Hash(string password);
    bool Verify(string password, byte[] hash);
}
