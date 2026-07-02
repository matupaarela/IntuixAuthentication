# api-keys - Tasks

## Implementation Tasks

### Task 1: Create ApiKey Entity
- **Priority:** High
- **Status:** Pending
- **File:** `Domain/Entities/ApiKey.cs`
- **Changes:** Define entity
- **Validation:** Entity compiles

### Task 2: Create ApiKey DTOs
- **Priority:** High
- **Status:** Pending
- **File:** `Application/ApiKeys/DTOs/`
- **Changes:** ApiKeyGenerateRequest, ApiKeyGenerateResponse, ApiKeyResponse
- **Validation:** DTOs compile

### Task 3: Create IApiKeyRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Application/ApiKeys/Interfaces/IApiKeyRepository.cs`
- **Changes:** CRUD methods
- **Validation:** Interface compiles

### Task 4: Create ApiKeyConfiguration
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Configurations/ApiKeyConfiguration.cs`
- **Changes:** Configure table and indexes
- **Validation:** Configuration compiles

### Task 5: Add ApiKey to AuthDbContext
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/AuthDbContext.cs`
- **Changes:** Add DbSet and query filter
- **Validation:** DbContext compiles

### Task 6: Create Migration
- **Priority:** High
- **Status:** Pending
- **Changes:** Generate migration
- **Validation:** Migration applies

### Task 7: Implement ApiKeyRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/ApiKeyRepository.cs`
- **Changes:** Implement all methods
- **Validation:** Methods work

### Task 8: Create ApiKeyGenerateCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/ApiKeys/Commands/`
- **Changes:** Generate key, hash, store, return raw once
- **Validation:** Key generated

### Task 9: Create ApiKeyRevokeCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/ApiKeys/Commands/`
- **Changes:** Revoke key by setting IsActive = false
- **Validation:** Key revoked

### Task 10: Create ApiKeyGetListQuery
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/ApiKeys/Queries/`
- **Changes:** List keys with pagination
- **Validation:** Query works

### Task 11: Create ApiKeysController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/ApiKeysController.cs`
- **Changes:** CRUD endpoints with `APIKEY_MANAGE` permission
- **Validation:** Endpoints work

### Task 12: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/ApiKeys/`
- **Test Cases:** All handlers
- **Validation:** All tests pass

### Task 13: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/ApiKeysEndpointTests.cs`
- **Test Cases:** Full flow
- **Validation:** All tests pass

## Documentation Tasks

### Task 14: Document API Keys
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/api-keys.md`
- **Changes:** Document key generation, storage, and authentication flow
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] Key generation works
- [ ] Key returned once
- [ ] Key stored hashed
- [ ] Revocation works
- [ ] Tenant scoping enforced
- [ ] All unit tests pass
- [ ] All integration tests pass
