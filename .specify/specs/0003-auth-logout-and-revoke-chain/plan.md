# 0003-auth-logout-and-revoke-chain - Implementation Plan

## Architecture Design

```
AuthController.Logout()
  → IMediator.Send(LogoutCommand)
    → LogoutCommandHandler.Handle()
      → SHA256 hash
      → IRefreshTokenRepository.GetByHashAsync()
      → Set RevokedAt
      → Revoke chain (tokens with ReplacedByToken = this token)
    ← 200 OK

AuthController.LogoutAll()
  → IMediator.Send(LogoutAllCommand)
    → LogoutAllCommandHandler.Handle()
      → IRefreshTokenRepository.RevokeAllUserTokensAsync(userId)
    ← 200 OK
```

## Domain Changes

No domain changes required.

## Application Changes

### 1. Create LogoutCommand

**File:** `Application/Auth/Commands/Logout/LogoutCommand.cs`

```csharp
public record LogoutCommand(string RefreshToken) : IRequest;
```

### 2. Create LogoutCommandHandler

**File:** `Application/Auth/Commands/Logout/LogoutCommandHandler.cs`

Handler that:
- Hashes the refresh token
- Looks up the token
- Sets `RevokedAt`
- Recursively revokes chain via `ReplacedByToken`

### 3. Create LogoutAllCommand

**File:** `Application/Auth/Commands/Logout/LogoutAllCommand.cs`

```csharp
public record LogoutAllCommand : IRequest;
```

### 4. Create LogoutAllCommandHandler

**File:** `Application/Auth/Commands/Logout/LogoutAllCommandHandler.cs`

Handler that:
- Gets user ID from `ICurrentUser`
- Calls `RevokeAllUserTokensAsync(userId)`

### 5. Add Repository Methods

**File:** `Application/Auth/Interfaces/IRefreshTokenRepository.cs`

Add:
```csharp
Task RevokeAllUserTokensAsync(Guid userId, DateTime revokedAt);
```

### 6. Implement Repository Methods

**File:** `Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs`

Implement `RevokeAllUserTokensAsync`.

### 7. Update AuthController

**File:** `Api/Controllers/AuthController.cs`

- Replace stub `Revoke` with `Logout` command dispatch
- Add `LogoutAll` endpoint

## Infrastructure Changes

Implement `RevokeAllUserTokensAsync` in `RefreshTokenRepository`.

## API Changes

- `POST /auth/logout` → dispatches `LogoutCommand`
- `POST /auth/logout-all` → dispatches `LogoutAllCommand` (requires auth)

## Security Considerations

1. Logout must be idempotent
2. Logout-all must require authentication
3. Token chain must be fully revoked

## Migration Strategy

No migration required.

## Testing Strategy

### Unit Tests

1. `LogoutCommandHandlerTests`:
   - Valid token → revoked
   - Invalid token → idempotent (no error)
   - Chain revocation works

2. `LogoutAllCommandHandlerTests`:
   - All user tokens revoked

### Integration Tests

1. `LogoutEndpointTests`:
   - POST /auth/logout → 200
   - POST /auth/logout-all → 200
   - POST /auth/logout-all without auth → 401

## Rollback Strategy

1. Revert controller changes
2. Remove new command/handler files
3. No database changes

