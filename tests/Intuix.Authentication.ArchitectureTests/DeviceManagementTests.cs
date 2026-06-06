using Intuix.Authentication.Application.Devices.Commands;
using Intuix.Authentication.Application.Devices.Queries;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Intuix.Authentication.Infrastructure.Security;
using Intuix.Authentication.Infrastructure.Persistence;
using Intuix.Authentication.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Intuix.Authentication.ArchitectureTests;

public class DeviceManagementTests
{
    [Fact]
    public async Task Device_sessions_are_listed_and_current_session_marked()
    {
        using var scope = CreateScope();

        var handler = new DeviceGetListQueryHandler(scope.Repository, scope.CurrentUser);
        var result = await handler.Handle(new DeviceGetListQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.TokenId == scope.CurrentTokenId && x.IsCurrent);
        Assert.Contains(result, x => x.TokenId == scope.SecondTokenId && !x.IsCurrent);
        Assert.DoesNotContain(result, x => x.TokenId == scope.OtherUserTokenId);
    }

    [Fact]
    public async Task Revoke_session_marks_only_requested_token()
    {
        using var scope = CreateScope();

        var handler = new DeviceRevokeSessionCommandHandler(scope.Repository, scope.CurrentUser);
        await handler.Handle(new DeviceRevokeSessionCommand(scope.SecondTokenId), CancellationToken.None);

        var current = await scope.Context.RefreshTokens.FirstAsync(x => x.Id == scope.CurrentTokenId);
        var revoked = await scope.Context.RefreshTokens.FirstAsync(x => x.Id == scope.SecondTokenId);
        var otherUser = await scope.Context.RefreshTokens.FirstAsync(x => x.Id == scope.OtherUserTokenId);

        Assert.Null(current.RevokedAt);
        Assert.NotNull(revoked.RevokedAt);
        Assert.Null(otherUser.RevokedAt);
    }

    [Fact]
    public async Task Revoke_all_preserves_current_session()
    {
        using var scope = CreateScope();

        var handler = new DeviceRevokeAllSessionsCommandHandler(scope.Repository, scope.CurrentUser);
        await handler.Handle(new DeviceRevokeAllSessionsCommand(), CancellationToken.None);

        var current = await scope.Context.RefreshTokens.FirstAsync(x => x.Id == scope.CurrentTokenId);
        var other = await scope.Context.RefreshTokens.FirstAsync(x => x.Id == scope.SecondTokenId);
        var otherUser = await scope.Context.RefreshTokens.FirstAsync(x => x.Id == scope.OtherUserTokenId);

        Assert.Null(current.RevokedAt);
        Assert.NotNull(other.RevokedAt);
        Assert.Null(otherUser.RevokedAt);
    }

    [Fact]
    public void Jwt_provider_embeds_and_current_user_reads_session_id()
    {
        var sessionId = Guid.NewGuid();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super_secret_key_for_testing_123456",
                ["Jwt:Issuer"] = "auth-api",
                ["Jwt:Audience"] = "auth-client",
                ["Jwt:Expires"] = "15"
            })
            .Build();

        var provider = new JwtProvider(config);
        var token = provider.GenerateToken(
            new User { Id = Guid.NewGuid(), TenantId = Guid.NewGuid() },
            Guid.NewGuid(),
            sessionId,
            Array.Empty<string>(),
            Array.Empty<string>());

        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var sid = parsed.Claims.Single(x => x.Type == "sid").Value;

        Assert.Equal(sessionId.ToString(), sid);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("tenant", Guid.NewGuid().ToString()),
                new Claim("company", Guid.NewGuid().ToString()),
                new Claim("sid", sessionId.ToString())
            },
            "Bearer"));

        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var currentUser = new CurrentUser(accessor);

        Assert.Equal(sessionId, currentUser.RefreshTokenId);
    }

    private static Scope CreateScope()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var tenantId = Guid.NewGuid();
        var currentUser = new TestCurrentUser(tenantId);
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AuthDbContext(options, currentUser);
        context.Database.EnsureCreated();

        Seed(context, tenantId, currentUser);

        var repository = new RefreshTokenRepository(context);

        return new Scope(context, connection, repository, currentUser);
    }

    private static void Seed(AuthDbContext context, Guid tenantId, TestCurrentUser currentUser)
    {
        var now = DateTime.UtcNow;

        var userA = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Username = "device-user-a",
            Email = "device-user-a@example.com",
            PasswordHash = "hash-a",
            IsActive = true,
            IsLocked = false,
            FailedAttempts = 0,
            CreatedAt = now
        };

        var userB = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Username = "device-user-b",
            Email = "device-user-b@example.com",
            PasswordHash = "hash-b",
            IsActive = true,
            IsLocked = false,
            FailedAttempts = 0,
            CreatedAt = now
        };

        var currentToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userA.Id,
            User = userA,
            TokenHash = new byte[] { 1, 1, 1 },
            ExpiresAt = now.AddDays(7),
            CreatedAt = now.AddMinutes(-10),
            IpAddress = "10.0.0.1",
            UserAgent = "UA-A"
        };

        var secondToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userA.Id,
            User = userA,
            TokenHash = new byte[] { 2, 2, 2 },
            ExpiresAt = now.AddDays(7),
            CreatedAt = now.AddMinutes(-5),
            IpAddress = "10.0.0.2",
            UserAgent = "UA-B"
        };

        var otherUserToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userB.Id,
            User = userB,
            TokenHash = new byte[] { 3, 3, 3 },
            ExpiresAt = now.AddDays(7),
            CreatedAt = now.AddMinutes(-3),
            IpAddress = "10.0.0.3",
            UserAgent = "UA-C"
        };

        currentUser.UserId = userA.Id;
        currentUser.RefreshTokenId = currentToken.Id;

        context.AddRange(userA, userB, currentToken, secondToken, otherUserToken);
        context.SaveChanges();

        currentUser.CompanyId = Guid.NewGuid();
    }

    private sealed record Scope(AuthDbContext Context, SqliteConnection Connection, RefreshTokenRepository Repository, TestCurrentUser CurrentUser) : IDisposable
    {
        public Guid CurrentTokenId => CurrentUser.RefreshTokenId;
        public Guid SecondTokenId => Context.RefreshTokens.AsNoTracking().Where(x => x.UserId == CurrentUser.UserId).OrderBy(x => x.CreatedAt).Skip(1).Select(x => x.Id).First();
        public Guid OtherUserTokenId => Context.RefreshTokens.AsNoTracking().Where(x => x.UserId != CurrentUser.UserId).Select(x => x.Id).First();

        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }
    }

    private sealed class TestCurrentUser : ICurrentUser
    {
        public TestCurrentUser(Guid tenantId)
        {
            TenantId = tenantId;
            CompanyId = Guid.NewGuid();
        }

        public Guid UserId { get; set; }
        public Guid TenantId { get; private set; }
        public Guid CompanyId { get; set; }
        public Guid RefreshTokenId { get; set; }
        public bool IsAuthenticated { get; private set; } = true;

        public void SetTenant(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }
}
