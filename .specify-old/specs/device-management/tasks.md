# device-management - Tasks

## Implementation Tasks

### Task 1: Create Device DTOs
- **Priority:** High
- **Status:** Completed
- **File:** `Application/Devices/DTOs/DeviceSessionResponse.cs`
- **Changes:** TokenId, IpAddress, UserAgent, CreatedAt, IsCurrent
- **Validation:** DTO compiles

### Task 2: Extend IRefreshTokenRepository
- **Priority:** High
- **Status:** Completed
- **File:** `Application/Auth/Interfaces/IRefreshTokenRepository.cs`
- **Changes:**
  - `GetActiveSessionsByUserAsync(Guid userId)`
  - `RevokeSessionAsync(Guid tokenId, Guid userId)`
  - `RevokeAllSessionsExceptCurrentAsync(Guid userId, Guid currentTokenId)`
- **Validation:** Interface compiles

### Task 3: Implement Repository Methods
- **Priority:** High
- **Status:** Completed
- **File:** `Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs`
- **Changes:** Implement all new methods
- **Validation:** Methods work

### Task 4: Create DeviceGetListQuery and Handler
- **Priority:** High
- **Status:** Completed
- **Files:** `Application/Devices/Queries/`
- **Changes:** Return active sessions with device info
- **Validation:** Query works

### Task 5: Create DeviceRevokeSessionCommand and Handler
- **Priority:** High
- **Status:** Completed
- **Files:** `Application/Devices/Commands/`
- **Changes:** Revoke specific session, validate ownership
- **Validation:** Session revoked

### Task 6: Create DeviceRevokeAllSessionsCommand and Handler
- **Priority:** High
- **Status:** Completed
- **Files:** `Application/Devices/Commands/`
- **Changes:** Revoke all except current session
- **Validation:** All other sessions revoked

### Task 7: Create DevicesController
- **Priority:** High
- **Status:** Completed
- **File:** `Api/Controllers/DevicesController.cs`
- **Changes:** GET, DELETE, POST endpoints with `[Authorize]`
- **Validation:** Endpoints work

### Task 8: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Devices/`
- **Test Cases:** All handlers
- **Validation:** All tests pass

### Task 9: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/DevicesEndpointTests.cs`
- **Test Cases:** Full flow
- **Validation:** All tests pass

## Documentation Tasks

### Task 10: Document Device Management
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/device-management.md`
- **Changes:** Document session listing, revocation, and revoke-all
- **Validation:** Documentation is complete

## Validation Checkpoints

- [X] Session list works
- [X] Specific session revocation works
- [X] Revoke-all works
- [X] Current session preserved
- [X] User can only see own sessions
- [ ] All unit tests pass
- [ ] All integration tests pass
