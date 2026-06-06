# authorization-rbac - Tasks

## Implementation Tasks

### Task 1: Create Permission Constants
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Common/Constants/Permissions.cs`
- **Changes:** Define all permission code constants
- **Validation:** Constants compile

### Task 2: Audit All Controllers for Authorization
- **Priority:** High
- **Status:** Completed
- **Files:** All controllers in `Api/Controllers/`
- **Changes:**
  - Verify all endpoints have `[Authorize]` or `[Authorize(Policy = "...")]`
  - Add missing attributes
  - Verify `[AllowAnonymous]` on public endpoints
- **Validation:** All endpoints are properly secured

### Task 3: Create Permission Seed Data
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Scripts/PermissionsSeed.sql`
- **Changes:** SQL script to insert all permission codes
- **Validation:** Permissions exist in database

### Task 4: Assign Permissions to Admin Roles
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Scripts/AdminPermissionsSeed.sql`
- **Changes:** Assign all permissions to Administrador role
- **Validation:** Admin has all permissions

### Task 5: Write Architecture Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.ArchitectureTests/AuthorizationArchitectureTests.cs`
- **Test Cases:**
  - All controllers have authorization attributes
  - No endpoint is accessible without authentication
  - All permission policies resolve
- **Validation:** All architecture tests pass

### Task 6: Write Unit Tests for PermissionAuthorizationHandler
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Security/PermissionAuthorizationHandlerTests.cs`
- **Test Cases:**
  - Valid permission → context.Succeed()
  - Missing permission → context not succeeded
  - Multiple permissions (OR) → works
  - Pipe-separated permissions → works
- **Validation:** All tests pass

### Task 7: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/AuthorizationEndpointTests.cs`
- **Test Cases:**
  - No token → 401
  - Valid token + correct permission → 200
  - Valid token + missing permission → 403
  - Expired token → 401
- **Validation:** All tests pass

### Task 8: Document Authorization Architecture
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/authorization.md`
- **Changes:** Document the authorization flow, permission codes, and how to add new permissions
- **Validation:** Documentation is complete

## Validation Checkpoints

- [X] All endpoints have authorization
- [ ] Permission constants defined
- [ ] Seed data created
- [ ] Architecture tests pass
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Documentation complete
