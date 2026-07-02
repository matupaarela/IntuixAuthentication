using Intuix.Authentication.Application.Auth.Commands.Login;
using Intuix.Authentication.Application.Auth.Commands.RefreshToken;
using Intuix.Authentication.Application.Auth.DTOs;
using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Intuix.Authentication.Domain.Interfaces;
using Intuix.Authentication.Infrastructure.Persistence;
using Intuix.Authentication.Infrastructure.Persistence.Repositories;
using Intuix.Authentication.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Intuix.Authentication.ArchitectureTests;

public class AuthHardeningTests
{
    [Fact]
    public async Task Login_persists_failed_attempts_and_locks_after_threshold()
    {
        using var scope = CreateScope();

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                scope.LoginHandler.Handle(
                    new LoginCommand(scope.Username, "wrong-password", scope.TenantCode, "127.0.0.1", "UA"),
                    CancellationToken.None));

            var user = await scope.Context.Users.AsNoTracking().SingleAsync(x => x.Id == scope.UserId);

            Assert.Equal(attempt, user.FailedAttempts);
            Assert.Equal(attempt >= 5, user.IsLocked);
        }
    }

    [Fact]
    public async Task Successful_login_persists_last_login_and_last_used_at()
    {
        using var scope = CreateScope();

        var result = await scope.LoginHandler.Handle(
            new LoginCommand(scope.Username, scope.Password, scope.TenantCode, "127.0.0.1", "UA"),
            CancellationToken.None);

        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(scope.CompanyId, result.CompanyId);
        Assert.Equal(scope.UserId, result.UserId);
        Assert.Equal(scope.TenantId, result.TenantId);

        var user = await scope.Context.Users.AsNoTracking().SingleAsync(x => x.Id == scope.UserId);
        var token = await scope.Context.RefreshTokens.AsNoTracking().SingleAsync(x => x.UserId == scope.UserId);

        Assert.NotNull(user.LastLogin);
        Assert.False(user.IsLocked);
        Assert.Equal(0, user.FailedAttempts);
        Assert.Equal(token.CreatedAt, token.LastUsedAt);
        Assert.Equal("127.0.0.1", token.IpAddress);
        Assert.Equal("UA", token.UserAgent);
    }

    [Fact]
    public async Task Refresh_reuse_revokes_the_session_family()
    {
        using var scope = CreateScope();

        var login = await scope.LoginHandler.Handle(
            new LoginCommand(scope.Username, scope.Password, scope.TenantCode, "127.0.0.1", "UA"),
            CancellationToken.None);

        var rotated = await scope.RefreshHandler.Handle(
            new RefreshTokenCommand(login.RefreshToken, "127.0.0.1", "UA"),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            scope.RefreshHandler.Handle(
                new RefreshTokenCommand(login.RefreshToken, "127.0.0.1", "UA"),
                CancellationToken.None));

        var tokens = await scope.Context.RefreshTokens
            .AsNoTracking()
            .Where(x => x.UserId == scope.UserId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        Assert.Equal(2, tokens.Count);
        Assert.NotEqual(login.RefreshToken, rotated.RefreshToken);
        Assert.All(tokens, x => Assert.NotNull(x.RevokedAt));
    }

    private static Scope CreateScope()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var tenantId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var tenantCode = "TENANT-A";
        var username = "alice";
        var password = "Correct123!";

        var currentUser = new TestCurrentUser();
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AuthDbContext(options, currentUser);
        context.Database.EnsureCreated();

        var hasher = new PasswordHasher();
        var jwtProvider = new JwtProvider(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super_secret_key_for_testing_123456",
                ["Jwt:Issuer"] = "auth-api",
                ["Jwt:Audience"] = "auth-client",
                ["Jwt:Expires"] = "15"
            })
            .Build());

        var tenant = new Tenant
        {
            Id = tenantId,
            Code = tenantCode,
            Name = "Tenant A",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var organization = new Organization
        {
            Id = organizationId,
            TenantId = tenantId,
            Name = "Org A",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        organization.Tenant = tenant;

        var company = new Company
        {
            Id = companyId,
            OrganizationId = organizationId,
            Name = "Company A",
            IsActive = true
        };
        company.Organization = organization;

        var user = new User
        {
            Id = userId,
            TenantId = tenantId,
            Username = username,
            Email = "alice@example.com",
            PasswordHash = Convert.ToBase64String(hasher.Hash(password)),
            IsActive = true,
            IsLocked = false,
            FailedAttempts = 0,
            CreatedAt = DateTime.UtcNow
        };

        var userCompany = new UserCompany
        {
            UserId = userId,
            CompanyId = companyId,
            IsDefault = true,
            User = user,
            Company = company
        };

        context.AddRange(tenant, organization, company, user, userCompany);
        context.SaveChanges();

        var tenantContext = currentUser;
        var tenantRepo = new TenantRepository(context);
        var userRepo = new UserRepository(context, tenantContext);
        var refreshRepo = new RefreshTokenRepository(context);
        var refreshService = new RefreshTokenService();
        var loginHandler = new LoginCommandHandler(
            userRepo,
            refreshRepo,
            hasher,
            jwtProvider,
            refreshService,
            tenantRepo,
            tenantContext);
        var refreshHandler = new RefreshTokenCommandHandler(
            refreshRepo,
            userRepo,
            jwtProvider,
            refreshService,
            tenantContext);

        return new Scope(
            context,
            connection,
            loginHandler,
            refreshHandler,
            tenantContext,
            tenantId,
            companyId,
            userId,
            tenantCode,
            username,
            password);
    }

    private sealed record Scope(
        AuthDbContext Context,
        SqliteConnection Connection,
        LoginCommandHandler LoginHandler,
        RefreshTokenCommandHandler RefreshHandler,
        TestCurrentUser TenantContext,
        Guid TenantId,
        Guid CompanyId,
        Guid UserId,
        string TenantCode,
        string Username,
        string Password) : IDisposable
    {
        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }
    }

    private sealed class TestCurrentUser : ICurrentUser, ITenantContext
    {
        public Guid UserId { get; private set; }
        public Guid TenantId { get; private set; }
        public Guid CompanyId { get; private set; }
        public Guid RefreshTokenId { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public void SetTenant(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }
}
