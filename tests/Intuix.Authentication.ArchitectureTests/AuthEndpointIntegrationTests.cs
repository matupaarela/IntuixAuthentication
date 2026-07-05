using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Intuix.Authentication.Infrastructure.Persistence;
using Intuix.Authentication.Infrastructure.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Intuix.Authentication.ArchitectureTests;

public class AuthEndpointIntegrationTests
{
    [Fact]
    public async Task Login_endpoint_accepts_seeded_admin_credentials()
    {
        using var factory = new AuthApiFactory();
        await factory.InitializeAsync();

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("IntuixAuthIntegration/1.0");

        var response = await client.PostAsJsonAsync("/auth/login", new
        {
            tenantCode = "TNT-INTUIX",
            username = "admin",
            password = "Admin123!"
        });

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(body);
        Assert.NotEmpty(body!.AccessToken);
        Assert.NotEmpty(body.RefreshToken);
        Assert.Equal(factory.TenantId, body.TenantId);
        Assert.Equal(factory.CompanyId, body.CompanyId);
        Assert.Equal(factory.UserId, body.UserId);
        Assert.True(body.ExpiresAt > DateTime.UtcNow);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var token = await db.RefreshTokens.AsNoTracking().SingleAsync(x => x.UserId == factory.UserId);

        Assert.Equal(token.CreatedAt, token.LastUsedAt);
        Assert.Equal("IntuixAuthIntegration/1.0", token.UserAgent);
        Assert.Null(token.RevokedAt);
    }

    private sealed record LoginResponse(
        [property: JsonPropertyName("accessToken")] string AccessToken,
        [property: JsonPropertyName("refreshToken")] string RefreshToken,
        [property: JsonPropertyName("expiresAt")] DateTime ExpiresAt,
        [property: JsonPropertyName("userId")] Guid UserId,
        [property: JsonPropertyName("tenantId")] Guid TenantId,
        [property: JsonPropertyName("companyId")] Guid CompanyId);

    private sealed class AuthApiFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection = new("DataSource=:memory:;Cache=Shared");
        private readonly TestTenantContext _tenantContext = new();
        private readonly PasswordHasher _passwordHasher = new();

        public AuthApiFactory()
        {
            _connection.Open();
        }

        public Guid TenantId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public Guid OrganizationId { get; } = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public Guid CompanyId { get; } = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public Guid UserId { get; } = Guid.Parse("44444444-4444-4444-4444-444444444444");

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Default"] = "DataSource=ignored",
                    ["Jwt:Key"] = "super_secret_key_for_testing_123456",
                    ["Jwt:Issuer"] = "auth-api",
                    ["Jwt:Audience"] = "auth-client",
                    ["Jwt:Expires"] = "15"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AuthDbContext>));
                services.RemoveAll<AuthDbContext>();
                services.RemoveAll<ITenantContext>();

                services.AddSingleton<ITenantContext>(_tenantContext);
                services.AddDbContext<AuthDbContext>(options => options.UseSqlite(_connection));
            });
        }

        public async Task InitializeAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            await db.Database.EnsureCreatedAsync();

            if (await db.Tenants.AnyAsync(x => x.Code == "TNT-INTUIX"))
                return;

            var now = DateTime.UtcNow;

            var tenant = new Tenant
            {
                Id = TenantId,
                Name = "Intuix Holding",
                Code = "TNT-INTUIX",
                IsActive = true,
                CreatedAt = now
            };

            var organization = new Organization
            {
                Id = OrganizationId,
                TenantId = TenantId,
                Name = "Intuix Corp",
                IsActive = true,
                CreatedAt = now
            };
            organization.Tenant = tenant;

            var company = new Company
            {
                Id = CompanyId,
                OrganizationId = OrganizationId,
                Name = "Intuix Software SAC",
                IsActive = true
            };
            company.Organization = organization;

            var user = new User
            {
                Id = UserId,
                TenantId = TenantId,
                Username = "admin",
                Email = "admin@intuix.com",
                PasswordHash = Convert.ToBase64String(_passwordHasher.Hash("Admin123!")),
                IsActive = true,
                IsLocked = false,
                FailedAttempts = 0,
                CreatedAt = now
            };

            var membership = new UserCompany
            {
                UserId = UserId,
                CompanyId = CompanyId,
                IsDefault = true,
                User = user,
                Company = company
            };

            db.AddRange(tenant, organization, company, user, membership);
            await db.SaveChangesAsync();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _connection.Dispose();
        }

        private sealed class TestTenantContext : ITenantContext
        {
            public Guid TenantId { get; private set; }

            public void SetTenant(Guid tenantId)
            {
                TenantId = tenantId;
            }
        }
    }
}
