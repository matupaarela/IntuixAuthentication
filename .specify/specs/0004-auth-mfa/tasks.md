# 0004-auth-mfa - Tasks

## Implementation Tasks

### Task 1: Add MFA Fields to User Entity
- **Priority:** High
- **Status:** Pending
- **File:** `Domain/Entities/User.cs`
- **Changes:** Add MfaEnabled, MfaSecret, BackupCodes
- **Validation:** Entity compiles

### Task 2: Create MFA DTOs
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Mfa/DTOs/`
- **Changes:** MfaEnableResponse, MfaStatusResponse
- **Validation:** DTOs compile

### Task 3: Create IMfaService Interface
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Common/Interfaces/IMfaService.cs`
- **Changes:** TOTP generation, verification, backup codes
- **Validation:** Interface compiles

### Task 4: Implement MfaService
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Security/MfaService.cs`
- **Changes:** Implement TOTP using HMAC-SHA1
- **Validation:** TOTP works

### Task 5: Update UserConfiguration
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Configurations/UserConfiguration.cs`
- **Changes:** Add MFA columns
- **Validation:** Configuration compiles

### Task 6: Create Migration
- **Priority:** High
- **Status:** Pending
- **Changes:** Add MFA columns to auth_users
- **Validation:** Migration applies

### Task 7: Create MfaEnableCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Mfa/Commands/`
- **Changes:** Generate secret, return QR URL and backup codes
- **Validation:** Secret generated

### Task 8: Create MfaVerifySetupCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Mfa/Commands/`
- **Changes:** Verify TOTP code, activate MFA
- **Validation:** MFA activated

### Task 9: Create MfaDisableCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Mfa/Commands/`
- **Changes:** Verify password, disable MFA
- **Validation:** MFA disabled

### Task 10: Create MfaGetStatusQuery
- **Priority:** Medium
- **Status:** Pending
- **Files:** `Application/Mfa/Queries/`
- **Changes:** Return MFA status
- **Validation:** Query works

### Task 11: Create MfaController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/MfaController.cs`
- **Changes:** All endpoints with `[Authorize]`
- **Validation:** Endpoints work

### Task 12: Integrate MFA into Login
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Auth/Commands/Login/LoginCommandHandler.cs`
- **Changes:**
  - If MFA enabled, require TOTP code
  - Verify TOTP before issuing tokens
- **Validation:** MFA login works

### Task 13: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Mfa/`
- **Test Cases:** TOTP generation, verification, backup codes
- **Validation:** All tests pass

### Task 14: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/MfaEndpointTests.cs`
- **Test Cases:** Full MFA flow
- **Validation:** All tests pass

## Documentation Tasks

### Task 15: Document MFA
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/0004-auth-mfa.md`
- **Changes:** Document TOTP setup, verification, backup codes, and login flow
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] TOTP generation works
- [ ] TOTP verification works
- [ ] Backup codes work
- [ ] MFA login flow works
- [ ] MFA disable works
- [ ] Secrets encrypted
- [ ] All unit tests pass
- [ ] All integration tests pass

