# 0003-auth-logout-and-revoke-chain - Tasks

## Implementation Tasks

### Task 1: Create LogoutCommand and Handler
- **Priority:** High
- **Status:** Completed
- **Files:**
  - `Application/Auth/Commands/Logout/LogoutCommand.cs`
  - `Application/Auth/Commands/Logout/LogoutCommandHandler.cs`
- **Changes:**
  - `LogoutCommand(string RefreshToken) : IRequest<Unit>`
  - Handler: hash → lookup → revoke → revoke chain
- **Validation:** Token is revoked, chain is revoked

### Task 2: Create LogoutAllCommand and Handler
- **Priority:** High
- **Status:** Completed
- **Files:**
  - `Application/Auth/Commands/Logout/LogoutAllCommand.cs`
  - `Application/Auth/Commands/Logout/LogoutAllCommandHandler.cs`
- **Changes:**
  - `LogoutAllCommand() : IRequest<Unit>`
  - Handler: get userId → revoke all tokens
- **Validation:** All user tokens revoked

### Task 3: Add RevokeAllUserTokensAsync to Repository
- **Priority:** High
- **Status:** Completed
- **Files:**
  - `Application/Auth/Interfaces/IRefreshTokenRepository.cs`
  - `Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs`
- **Changes:**
  - Interface: `Task RevokeTokenChainAsync(Guid tokenId, string revocationReason, DateTime revokedAt);`
  - Interface: `Task RevokeAllUserTokensAsync(Guid userId, string revocationReason, DateTime revokedAt);`
  - Implementation: query active tokens, set RevokedAt, SaveChanges
- **Validation:** All active tokens for user are revoked

### Task 4: Update AuthController
- **Priority:** High
- **Status:** Completed
- **File:** `Api/Controllers/AuthController.cs`
- **Changes:**
  - Replace `Revoke` stub with `Logout` endpoint dispatching `LogoutCommand`
  - Add `LogoutAll` endpoint dispatching `LogoutAllCommand` with `[Authorize]`
- **Validation:** Endpoints return correct responses

### Task 5: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **Files:**
  - `tests/Intuix.Authentication.UnitTests/Auth/Commands/LogoutCommandHandlerTests.cs`
  - `tests/Intuix.Authentication.UnitTests/Auth/Commands/LogoutAllCommandHandlerTests.cs`
- **Test Cases:**
  - Valid token → revoked
  - Invalid token → no error (idempotent)
  - Chain revocation works
  - All user tokens revoked on logout-all
- **Validation:** All tests pass

### Task 6: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/LogoutEndpointTests.cs`
- **Test Cases:**
  - POST /auth/logout → 200
  - POST /auth/logout-all → 200
  - POST /auth/logout-all without auth → 401
  - POST /auth/logout with revoked token → 200
- **Validation:** All tests pass

## Documentation Tasks

### Task 7: Document Logout Flow
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/0003-auth-logout-and-revoke-chain.md`
- **Changes:** Document single logout, logout-all, and chain revocation
- **Validation:** Documentation is complete

## Validation Checkpoints

- [X] Single logout revokes token
- [X] Logout is idempotent
- [X] Chain revocation works
- [X] Logout-all revokes all user tokens
- [X] Logout-all requires authentication
- [ ] All unit tests pass
- [ ] All integration tests pass

