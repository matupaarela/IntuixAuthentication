# 0012-user-management - Tasks

## Implementation Tasks

### Task 1: Create User DTOs
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Users/DTOs/`
- **Changes:**
  - `UserResponse`: Id, Username, Email, IsActive, IsLocked, LastLogin, CreatedAt
  - `UserCreateRequest`: Username, Email, Password
  - `UserRoleResponse`: RoleId, RoleName
  - `UserCompanyResponse`: CompanyId, CompanyName, IsDefault
- **Validation:** DTOs compile, no password hash in response

### Task 2: Extend IUserRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Auth/Interfaces/IUserRepository.cs` (move to `Application/Users/Interfaces/`)
- **Changes:** Add all new methods
- **Validation:** Interface compiles

### Task 3: Implement UserRepository Methods
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/UserRepository.cs`
- **Changes:** Implement all new methods with tenant scoping
- **Validation:** Methods work

### Task 4: Create UserCreateCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Users/Commands/UserCreateCommand.cs`, `UserCreateCommandHandler.cs`
- **Changes:**
  - Validate username uniqueness within tenant
  - Hash password with `IPasswordHasher`
  - Create user with tenant ID from `ICurrentUser`
- **Validation:** User created with hashed password

### Task 5: Create UserUpdateCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Users/Commands/UserUpdateCommand.cs`, `UserUpdateCommandHandler.cs`
- **Changes:** Update email only (username is identifier)
- **Validation:** User updated

### Task 6: Create UserResetPasswordCommand and Handler
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Users/Commands/UserResetPasswordCommand.cs`, `UserResetPasswordCommandHandler.cs`
- **Changes:**
  - Hash new password with `IPasswordHasher`
  - Update password hash
  - Reset failed attempts
  - Unlock account if locked
- **Validation:** Password reset, account unlocked

### Task 7: Create Lock/Unlock Commands
- **Priority:** Medium
- **Status:** Pending
- **Files:** `Application/Users/Commands/UserLockCommand.cs`, `UserUnlockCommand.cs`
- **Changes:** Toggle IsLocked flag
- **Validation:** Lock/unlock works

### Task 8: Create UserRole Commands
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Users/Commands/UserRoleAssignCommand.cs`, `UserRoleRemoveCommand.cs`
- **Changes:** Assign/remove role within tenant
- **Validation:** Role assignment works

### Task 9: Create User Queries
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Users/Queries/`
- **Changes:** GetList, GetById, GetRoles, GetCompanies
- **Validation:** Queries work

### Task 10: Create UsersController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/UsersController.cs`
- **Changes:** All endpoints with `USER_MANAGE` permission
- **Validation:** Endpoints work

### Task 11: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Users/`
- **Test Cases:** All handlers
- **Validation:** All tests pass

### Task 12: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/UsersEndpointTests.cs`
- **Test Cases:** Full flow
- **Validation:** All tests pass

## Documentation Tasks

### Task 13: Document User Management
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/0012-user-management.md`
- **Changes:** Document user CRUD, password management, role assignment, and account status
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] Password hashing works
- [ ] Password hash never in responses
- [ ] Tenant scoping enforced
- [ ] Role assignment works
- [ ] Lock/unlock works
- [ ] All unit tests pass
- [ ] All integration tests pass

