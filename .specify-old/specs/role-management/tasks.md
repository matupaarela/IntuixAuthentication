# role-management - Tasks

## Implementation Tasks

### Task 1: Create Role DTOs
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Roles/DTOs/`
- **Changes:**
  - `RoleResponse`: Id, Name, PermissionCount
  - `RoleDetailResponse`: Id, Name, Permissions (list)
  - `RoleCreateRequest`: Name
  - `RoleUpdateRequest`: Name
  - `RoleAssignPermissionRequest`: PermissionId
- **Validation:** DTOs compile

### Task 2: Create IRoleRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Roles/Interfaces/IRoleRepository.cs`
- **Changes:** CRUD + permission assignment methods
- **Validation:** Interface compiles

### Task 3: Implement RoleRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/RoleRepository.cs`
- **Changes:** All methods with tenant scoping
- **Validation:** Methods work

### Task 4: Create RoleCreateCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Roles/Commands/`
- **Changes:** Validate name uniqueness within tenant, create role
- **Validation:** Role created

### Task 5: Create RoleUpdateCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Roles/Commands/`
- **Changes:** Validate name uniqueness, update role
- **Validation:** Role updated

### Task 6: Create RoleAssignPermissionCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Roles/Commands/`
- **Changes:** Validate permission exists, assign to role
- **Validation:** Permission assigned

### Task 7: Create RoleRemovePermissionCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Roles/Commands/`
- **Changes:** Remove permission from role
- **Validation:** Permission removed

### Task 8: Create Role Queries
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Roles/Queries/`
- **Changes:** GetList and GetById with permissions
- **Validation:** Queries work

### Task 9: Create RolesController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/RolesController.cs`
- **Changes:** All endpoints with `ROLE_MANAGE` permission
- **Validation:** Endpoints work

### Task 10: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Roles/`
- **Test Cases:** All handlers
- **Validation:** All tests pass

### Task 11: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/RolesEndpointTests.cs`
- **Test Cases:** Full flow
- **Validation:** All tests pass

## Documentation Tasks

### Task 12: Document Role Management
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/role-management.md`
- **Changes:** Document role CRUD, permission assignment, and tenant scoping
- **Validation:** Documentation is complete

## Validation Checkpoints

- [X] Tenant scoping enforced
- [X] Name uniqueness enforced
- [ ] Permission assignment works
- [ ] All unit tests pass
- [ ] All integration tests pass
