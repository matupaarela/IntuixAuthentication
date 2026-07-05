using Intuix.Authentication.Api.Middleware;
using Intuix.Authentication.Application.Auth.Commands.Login;
using Intuix.Authentication.Application.Auth.Commands.Logout;
using Intuix.Authentication.Application.Auth.Commands.RefreshToken;
using Intuix.Authentication.Application.Auth.Commands.SwitchCompany;
using Intuix.Authentication.Application.Auth.Validators;
using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Behaviors;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Application.Devices.Commands;
using Intuix.Authentication.Application.Devices.Queries;
using Intuix.Authentication.Application.Devices.Validators;
using Intuix.Authentication.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Intuix.Authentication.Infrastructure.Persistence;
using Intuix.Authentication.Infrastructure.Persistence.Repositories;
using Intuix.Authentication.Infrastructure.Security;
using Intuix.Authentication.Infrastructure.Security.Authorization;
using Intuix.Authentication.Api.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var key = builder.Configuration["Jwt:Key"]; // misma key que usas al generar token

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "auth-api",
            ValidAudience = "auth-client",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!)
            ),

            ClockSkew = TimeSpan.Zero // importante para expiraci�n exacta
        };
    });


builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SecurityLoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
builder.Services.AddScoped<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();
builder.Services.AddScoped<IValidator<LogoutCommand>, LogoutCommandValidator>();
builder.Services.AddScoped<IValidator<LogoutAllCommand>, LogoutAllCommandValidator>();
builder.Services.AddScoped<IValidator<SwitchCompanyCommand>, SwitchCompanyCommandValidator>();
builder.Services.AddScoped<IValidator<DeviceGetListQuery>, DeviceGetListQueryValidator>();
builder.Services.AddScoped<IValidator<DeviceRevokeSessionCommand>, DeviceRevokeSessionCommandValidator>();
builder.Services.AddScoped<IValidator<DeviceRevokeAllSessionsCommand>, DeviceRevokeAllSessionsCommandValidator>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<ITenantContext>(sp =>
    sp.GetRequiredService<ICurrentUser>() as ITenantContext
        ?? throw new InvalidOperationException("CurrentUser must implement ITenantContext."));

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
    });

    options.OperationFilter<AuthorizeOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseMiddleware<TenantMiddleware>();

app.UseAuthorization();

app.MapControllers();




//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
//    await db.Database.MigrateAsync();
//    await AuthDbSeeder.SeedAsync(db);
//}

app.Run();

public partial class Program { }
