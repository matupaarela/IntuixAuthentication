# 0001-auth-login - Implementation Plan

## Architecture Design

The login feature follows CQRS pattern with a single `LoginCommand` handled by `LoginCommandHandler`. The handler orchestrates: tenant resolution → user lookup → credential validation → token generation → refresh token persistence.

```
AuthController.Login()
  → IMediator.Send(LoginCommand)
    → LoginCommandHandler.Handle()
      → ITenantRepository.GetByCodeAsync()
      → ICurrentUser.SetTenant()
      → IUserRepository.GetByUsernameAsync()
      → IPasswordHasher.Verify()
      → IJwtProvider.GenerateToken()
      → IRefreshTokenService.Generate()
      → IRefreshTokenRepository.AddAsync()
    ← AuthResponse
  ← 200 OK
```

## Domain Changes

No domain entity changes required. The `User` entity already has all required fields (`FailedAttempts`, `IsLocked`, `LastLogin`, `PasswordHash`).

## Application Changes

### 1. Fix LoginCommandHandler

**File:** `Application/Auth/Commands/Login/LoginCommandHandler.cs`

Changes:
- Replace `request.Password != user.PasswordHash` with `_hasher.Verify(request.Password, user.PasswordHash)`
- Ensure `_hasher` is injected (already is)
- Ensure proper error handling

### 2. LoginCommand Already Defined

**File:** `Application/Auth/Commands/Login/LoginCommand.cs`

Already has `Username`, `Password`, `TenantCode` properties. No changes needed.

### 3. AuthResponse Already Defined

**File:** `Application/Auth/DTOs/AuthResponse.cs`

Already has all required fields. No changes needed.

## Infrastructure Changes

No infrastructure changes required. `PasswordHasher`, `JwtProvider`, `RefreshTokenService`, and all repositories are already implemented.

## API Changes

No API changes required. `AuthController.Login()` already dispatches `LoginCommand` and returns `AuthResponse`.

## Security Considerations

1. Password comparison must use `IPasswordHasher.Verify()` - currently uses plain text comparison
2. Error messages must be generic to prevent credential enumeration
3. Failed login attempts must be logged
4. Account lockout after 5 attempts must be enforced

## Migration Strategy

No database migration required. All tables and columns already exist.

## Testing Strategy

### Unit Tests

1. `LoginCommandHandlerTests`:
   - `Handle_ValidCredentials_ReturnsAuthResponse`
   - `Handle_InvalidUsername_ThrowsException`
   - `Handle_InvalidPassword_ThrowsException`
   - `Handle_LockedAccount_ThrowsException`
   - `Handle_InactiveUser_ThrowsException`
   - `Handle_FailedAttempts_IncrementsCounter`
   - `Handle_FifthFailedAttempt_LocksAccount`
   - `Handle_SuccessfulLogin_ResetsFailedAttempts`
   - `Handle_SuccessfulLogin_UpdatesLastLogin`
   - `Handle_InvalidTenant_ThrowsException`

### Integration Tests

1. `LoginEndpointTests`:
   - `Login_ValidCredentials_Returns200WithTokens`
   - `Login_InvalidCredentials_Returns400`
   - `Login_LockedAccount_Returns400`
   - `Login_MissingFields_Returns400`

## Rollback Strategy

1. The change is a single line fix in `LoginCommandHandler`
2. If issues arise, revert the handler to previous version
3. No database changes to roll back
4. No infrastructure changes to roll back

