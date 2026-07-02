# 0002-auth-refresh-token-rotation - Implementation Plan

## Architecture Design

The refresh token rotation follows CQRS with `RefreshTokenCommand` handled by `RefreshTokenCommandHandler`. The handler performs: token lookup → validation → reuse detection → rotation → new JWT issuance.

```
AuthController.Refresh()
  → IMediator.Send(RefreshTokenCommand)
    → RefreshTokenCommandHandler.Handle()
      → SHA256 hash of token
      → IRefreshTokenRepository.GetByHashAsync()
      → Validate: not null, not expired, not revoked
      → If revoked: detect reuse → revoke family
      → Revoke current token (RevokedAt, ReplacedByToken)
      → IRefreshTokenService.Generate() → new token
      → IRefreshTokenRepository.AddAsync(new token)
      → IJwtProvider.GenerateToken()
    ← AuthResponse
  ← 200 OK
```

## Domain Changes

No domain entity changes. `RefreshToken` already has `RevokedAt`, `ReplacedByToken`, `IpAddress`, `UserAgent`.

## Application Changes

### 1. Enhance RefreshTokenCommandHandler

**File:** `Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs`

Changes:
- Add `IHttpContextAccessor` injection for IP/User-Agent
- Add `ILogger` injection for security logging
- Implement chain revocation on reuse detection
- Capture `IpAddress` and `UserAgent` on token creation
- Set `ReplacedByToken` correctly

### 2. Add Chain Revocation Method

**File:** `Application/Auth/Interfaces/IRefreshTokenRepository.cs`

Add method:
```csharp
Task RevokeTokenFamilyAsync(Guid userId, DateTime revokedAt);
```

### 3. Implement Chain Revocation

**File:** `Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs`

Implement `RevokeTokenFamilyAsync` to revoke all active tokens for a user in a single transaction.

## Infrastructure Changes

### 1. Update RefreshTokenRepository

Add `RevokeTokenFamilyAsync` implementation that:
- Queries all active tokens for the user
- Sets `RevokedAt` for each
- Executes in a single `SaveChangesAsync`

## API Changes

No API changes. Controller already dispatches `RefreshTokenCommand`.

## Security Considerations

1. Token reuse detection must log security warnings
2. Chain revocation must be atomic
3. IP and User-Agent must be captured for auditing
4. Token hash must use SHA256

## Migration Strategy

No migration required. `IpAddress` and `UserAgent` columns already exist.

## Testing Strategy

### Unit Tests

1. `RefreshTokenCommandHandlerTests`:
   - Valid token → returns new tokens
   - Expired token → throws
   - Revoked token → triggers chain revocation
   - Invalid token hash → throws
   - Inactive user → throws
   - Chain revocation revokes all user tokens

### Integration Tests

1. `RefreshEndpointTests`:
   - POST /auth/refresh with valid token → 200
   - POST /auth/refresh with expired token → 400
   - POST /auth/refresh with revoked token → 400

## Rollback Strategy

1. Revert handler changes
2. No database changes to roll back
3. Chain revocation is new functionality (additive)

