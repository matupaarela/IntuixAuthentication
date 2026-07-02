# oauth-sso - Tasks

## Implementation Tasks

### Task 1: Create ExternalLogin Entity
- **Priority:** High
- **Status:** Pending
- **File:** `Domain/Entities/ExternalLogin.cs`
- **Changes:** Define entity
- **Validation:** Entity compiles

### Task 2: Create SsoProvider Entity
- **Priority:** High
- **Status:** Pending
- **File:** `Domain/Entities/SsoProvider.cs`
- **Changes:** Define entity
- **Validation:** Entity compiles

### Task 3: Create SSO DTOs
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Sso/DTOs/`
- **Changes:** SsoRedirectResponse, SsoCallbackResponse
- **Validation:** DTOs compile

### Task 4: Create ISsoService Interface
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Common/Interfaces/ISsoService.cs`
- **Changes:** OAuth flow methods
- **Validation:** Interface compiles

### Task 5: Implement SsoService
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Security/SsoService.cs`
- **Changes:** Implement Google and Microsoft OAuth flows
- **Validation:** OAuth flows work

### Task 6: Create Entity Configurations
- **Priority:** High
- **Status:** Pending
- **Files:**
  - `Infrastructure/Persistence/Configurations/ExternalLoginConfiguration.cs`
  - `Infrastructure/Persistence/Configurations/SsoProviderConfiguration.cs`
- **Changes:** Configure tables and indexes
- **Validation:** Configurations compile

### Task 7: Add DbSets to AuthDbContext
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/AuthDbContext.cs`
- **Changes:** Add ExternalLogins and SsoProviders
- **Validation:** DbContext compiles

### Task 8: Create Migration
- **Priority:** High
- **Status:** Pending
- **Changes:** Create new tables
- **Validation:** Migration applies

### Task 9: Create SsoGetRedirectQuery
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Sso/Queries/`
- **Changes:** Generate redirect URL with state
- **Validation:** Query works

### Task 10: Create SsoCallbackCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Sso/Commands/`
- **Changes:**
  - Validate state
  - Exchange code
  - Find/create user
  - Link account
  - Issue tokens
- **Validation:** Callback works

### Task 11: Add SSO Endpoints to AuthController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/AuthController.cs`
- **Changes:**
  - `GET /auth/sso/{provider}` → redirect
  - `POST /auth/sso/{provider}/callback` → exchange
- **Validation:** Endpoints work

### Task 12: Seed SSO Provider Configurations
- **Priority:** Medium
- **Status:** Pending
- **File:** `Infrastructure/Scripts/SsoProvidersSeed.sql`
- **Changes:** Insert Google and Microsoft configs for development
- **Validation:** Seed data works

### Task 13: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Sso/`
- **Test Cases:** OAuth flow, state validation
- **Validation:** All tests pass

### Task 14: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/SsoEndpointTests.cs`
- **Test Cases:** Full SSO flow
- **Validation:** All tests pass

## Documentation Tasks

### Task 15: Document OAuth/SSO
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/oauth-sso.md`
- **Changes:** Document OAuth flow, provider configuration, auto-provisioning, and account linking
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] OAuth redirect works
- [ ] Code exchange works
- [ ] User auto-provisioning works
- [ ] Account linking works
- [ ] State validation works
- [ ] Multi-tenant SSO works
- [ ] All unit tests pass
- [ ] All integration tests pass
