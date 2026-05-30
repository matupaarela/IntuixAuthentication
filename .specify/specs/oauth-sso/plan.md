# oauth-sso - Implementation Plan

## Architecture Design

```
AuthController
  → GET /auth/sso/{provider} → SsoGetRedirectQuery
  → POST /auth/sso/{provider}/callback → SsoCallbackCommand

SsoCallbackCommandHandler
  → Validate state parameter
  → Exchange code for tokens with provider
  → Get user info from provider
  → Find or create local user
  → Link external account
  → Issue JWT + refresh token
```

## Domain Changes

### 1. Create ExternalLogin Entity

**File:** `Domain/Entities/ExternalLogin.cs`

```csharp
public class ExternalLogin
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = default!;
    public string ProviderUserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = default!;
}
```

### 2. Create SsoProvider Entity

**File:** `Domain/Entities/SsoProvider.cs`

```csharp
public class SsoProvider
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Provider { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public bool IsActive { get; set; }
}
```

## Application Changes

### 1. Create Feature Folder

```
Application/Sso/
├── Commands/
│   └── SsoCallbackCommand.cs/.Handler.cs
├── Queries/
│   └── SsoGetRedirectQuery.cs/.Handler.cs
├── DTOs/
│   ├── SsoRedirectResponse.cs
│   └── SsoCallbackResponse.cs
├── Validators/
└── Interfaces/
    └── ISsoService.cs
```

### 2. Create ISsoService

```csharp
public interface ISsoService
{
    string GenerateState();
    string GetRedirectUrl(string provider, string state, string tenantCode);
    Task<SsoUserInfo> ExchangeCodeAsync(string provider, string code, Guid tenantId);
    bool ValidateState(string state, string storedState);
}
```

## Infrastructure Changes

### 1. Create SsoService

**File:** `Infrastructure/Security/SsoService.cs`

Implement OAuth 2.0 flows for Google and Microsoft.

### 2. Create ExternalLoginConfiguration

### 3. Create SsoProviderConfiguration

### 4. Add DbSets to AuthDbContext

### 5. Create Migration

## API Changes

### 1. Add SSO endpoints to AuthController

## Security Considerations

1. State parameter validated (CSRF protection)
2. Client secrets encrypted at rest
3. Provider tokens not stored

## Migration Strategy

1. Create migration for new tables

## Testing Strategy

1. Unit tests for OAuth flow
2. Integration tests for callback

## Rollback Strategy

1. Remove new files
2. Drop migration
