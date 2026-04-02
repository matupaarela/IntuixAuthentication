namespace Intuix.Authentication.Domain.Interfaces;

public interface IRefreshTokenService
{
    (string token, byte[] hash) Generate();
}
