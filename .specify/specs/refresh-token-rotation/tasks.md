# refresh-token-rotation - Tasks

## Implementation Tasks

### Task 1: Add RevokeTokenFamilyAsync to IRefreshTokenRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Auth/Interfaces/IRefreshTokenRepository.cs`
- **Changes:** Add `Task RevokeTokenFamilyAsync(Guid userId, DateTime revokedAt);`
- **Validation:** Interface compiles

### Task 2: Implement RevokeTokenFamilyAsync in RefreshTokenRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs`
- **Changes:**
  - Query all active tokens for user (`RevokedAt == null`)
  - Set `RevokedAt` for each
  - Single `SaveChangesAsync` call
- **Validation:** All active tokens for user are revoked

### Task 3: Enhance RefreshTokenCommandHandler with Chain Revocation
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs`
- **Changes:**
  - Inject `IHttpContextAccessor` for IP/User-Agent
  - Inject `ILogger` for security logging
  - On reused token (revoked + not null): call `RevokeTokenFamilyAsync`
  - Log security warning on reuse detection
  - Capture `IpAddress` and `UserAgent` on new token
  - Set `ReplacedByToken` on old token
- **Validation:** Reuse detection triggers family revocation

### Task 4: Capture Device Metadata
- **Priority:** Medium
- **Status:** Pending
- **File:** `Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs`
- **Changes:**
  - Extract IP from `IHttpContextAccessor.HttpContext.Connection.RemoteIpAddress`
  - Extract User-Agent from `IHttpContextAccessor.HttpContext.Request.Headers.UserAgent`
  - Set on new `RefreshToken` entity
- **Validation:** IP and User-Agent stored in database

### Task 5: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Auth/Commands/RefreshTokenCommandHandlerTests.cs`
- **Test Cases:**
  - Valid token → new tokens returned
  - Expired token → throws
  - Revoked token → family revoked, throws
  - Invalid hash → throws
  - Inactive user → throws
  - Chain integrity verified
- **Validation:** All tests pass

### Task 6: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/RefreshEndpointTests.cs`
- **Test Cases:**
  - Valid refresh → 200 with new tokens
  - Expired refresh → 400
  - Revoked refresh → 400
  - Invalid refresh → 400
- **Validation:** All tests pass

### Task 7: Verify Chain Revocation
- **Priority:** Medium
- **Status:** Pending
- **Validation:**
  - Create token chain A → B → C
  - Revoke B manually
  - Use B → should trigger revocation of C
  - Verify all tokens in chain are revoked
- **Method:** Integration test

## Documentation Tasks

### Task 8: Document Refresh Token Rotation
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/refresh-token-rotation.md`
- **Changes:** Document the rotation flow, chain revocation, and reuse detection
- **Validation:** Documentation is complete

## Validation Checkpoints

- [X] Token rotation works correctly
- [X] Old token is revoked on rotation
- [X] ReplacedByToken is set correctly
- [X] Reuse detection works
- [ ] Chain revocation is atomic
- [ ] IP and User-Agent captured
- [X] Expired tokens rejected
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Security events logged
