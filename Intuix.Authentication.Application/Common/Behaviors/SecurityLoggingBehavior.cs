using Intuix.Authentication.Application.Auth.DTOs;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Application.Devices.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Intuix.Authentication.Application.Common.Behaviors;

public sealed class SecurityLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<SecurityLoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUser _currentUser;

    public SecurityLoggingBehavior(
        ILogger<SecurityLoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Starting {RequestName} tenant {TenantId} user {UserId} session {SessionId}",
            requestName,
            _currentUser.TenantId,
            _currentUser.UserId,
            _currentUser.RefreshTokenId);

        try
        {
            var response = await next();

            LogCompletion(requestName, response);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed {RequestName} tenant {TenantId} user {UserId} session {SessionId}",
                requestName,
                _currentUser.TenantId,
                _currentUser.UserId,
                _currentUser.RefreshTokenId);

            throw;
        }
    }

    private void LogCompletion(string requestName, TResponse response)
    {
        switch (response)
        {
            case AuthResponse authResponse:
                _logger.LogInformation(
                    "Completed {RequestName} tenant {TenantId} user {UserId} session {SessionId} company {CompanyId}",
                    requestName,
                    authResponse.TenantId != Guid.Empty ? authResponse.TenantId : _currentUser.TenantId,
                    authResponse.UserId != Guid.Empty ? authResponse.UserId : _currentUser.UserId,
                    _currentUser.RefreshTokenId,
                    authResponse.CompanyId);
                break;
            case IEnumerable<DeviceSessionResponse> sessions:
                _logger.LogInformation(
                    "Completed {RequestName} tenant {TenantId} user {UserId} session {SessionId} sessions {SessionCount}",
                    requestName,
                    _currentUser.TenantId,
                    _currentUser.UserId,
                    _currentUser.RefreshTokenId,
                    sessions.Count());
                break;
            default:
                _logger.LogInformation(
                    "Completed {RequestName} tenant {TenantId} user {UserId} session {SessionId}",
                    requestName,
                    _currentUser.TenantId,
                    _currentUser.UserId,
                    _currentUser.RefreshTokenId);
                break;
        }
    }
}
