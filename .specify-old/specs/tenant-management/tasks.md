# tenant-management - Tasks

## Implementation Tasks

### Task 1: Create Tenant DTOs
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Tenants/DTOs/TenantResponse.cs`
- **Changes:**
  - `TenantResponse`: Id, Name, Code, IsActive, CreatedAt
  - `TenantCreateRequest`: Name, Code
  - `TenantUpdateRequest`: Name, Code, IsActive
- **Validation:** DTOs compile

### Task 2: Extend ITenantRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Auth/Interfaces/ITenantRepository.cs` (move to `Application/Tenants/Interfaces/`)
- **Changes:**
  - `GetAllAsync(int page, int pageSize)`
  - `GetCountAsync()`
  - `GetByIdAsync(Guid id)`
  - `ExistsByCodeAsync(string code, Guid? excludeId)`
  - `AddAsync(Tenant tenant)`
  - `UpdateAsync(Tenant tenant)`
- **Validation:** Interface compiles

### Task 3: Implement TenantRepository Methods
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/TenantRepository.cs`
- **Changes:** Implement all new methods
- **Validation:** Methods work correctly

### Task 4: Create TenantCreateCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:**
  - `Application/Tenants/Commands/TenantCreateCommand.cs`
  - `Application/Tenants/Commands/TenantCreateCommandHandler.cs`
- **Changes:** Validate code uniqueness, create tenant
- **Validation:** Tenant created with correct fields

### Task 5: Create TenantUpdateCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:**
  - `Application/Tenants/Commands/TenantUpdateCommand.cs`
  - `Application/Tenants/Commands/TenantUpdateCommandHandler.cs`
- **Changes:** Validate code uniqueness, update tenant
- **Validation:** Tenant updated

### Task 6: Create TenantGetListQuery and Handler
- **Priority:** High
- **Status:** Pending
- **Files:**
  - `Application/Tenants/Queries/TenantGetListQuery.cs`
  - `Application/Tenants/Queries/TenantGetListQueryHandler.cs`
- **Changes:** Paginated list with total count
- **Validation:** Returns correct page

### Task 7: Create TenantGetByIdQuery and Handler
- **Priority:** High
- **Status:** Pending
- **Files:**
  - `Application/Tenants/Queries/TenantGetByIdQuery.cs`
  - `Application/Tenants/Queries/TenantGetByIdQueryHandler.cs`
- **Changes:** Get tenant by ID
- **Validation:** Returns tenant or null

### Task 8: Create Validators
- **Priority:** Medium
- **Status:** Pending
- **Files:**
  - `Application/Tenants/Validators/TenantCreateCommandValidator.cs`
  - `Application/Tenants/Validators/TenantUpdateCommandValidator.cs`
- **Changes:** FluentValidation rules
- **Validation:** Validation works

### Task 9: Create TenantsController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/TenantsController.cs`
- **Changes:**
  - `GET /api/tenants` → list
  - `GET /api/tenants/{id}` → get by id
  - `POST /api/tenants` → create
  - `PUT /api/tenants/{id}` → update
  - All with `[Authorize(Policy = "TENANT_MANAGE")]`
- **Validation:** Endpoints work

### Task 10: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Tenants/Commands/`
- **Test Cases:**
  - Create valid → success
  - Create duplicate code → throws
  - Update valid → success
  - Update duplicate code → throws
- **Validation:** All tests pass

### Task 11: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/TenantsEndpointTests.cs`
- **Test Cases:**
  - Full CRUD flow
  - Permission checks
  - Validation errors
- **Validation:** All tests pass

## Documentation Tasks

### Task 12: Document Tenant Management
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/tenant-management.md`
- **Changes:** Document tenant CRUD operations and code uniqueness rules
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] CRUD operations work
- [X] Code uniqueness enforced
- [ ] Permission checks work
- [ ] Pagination works
- [ ] All unit tests pass
- [ ] All integration tests pass
