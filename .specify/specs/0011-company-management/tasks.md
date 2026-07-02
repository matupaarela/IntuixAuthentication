# 0011-company-management - Tasks

## Implementation Tasks

### Task 1: Create Company DTOs
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Companies/DTOs/`
- **Changes:** CompanyResponse, CompanyCreateRequest, CompanyUpdateRequest, CompanyAssignUserRequest
- **Validation:** DTOs compile

### Task 2: Create ICompanyRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Companies/Interfaces/ICompanyRepository.cs`
- **Changes:** CRUD + user assignment methods
- **Validation:** Interface compiles

### Task 3: Implement CompanyRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/CompanyRepository.cs`
- **Changes:** All methods with organization scoping
- **Validation:** Methods work

### Task 4: Create Commands and Handlers
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Companies/Commands/`
- **Changes:** Create, Update, AssignUser, RemoveUser, SetDefaultUser
- **Validation:** Commands work

### Task 5: Create Queries and Handlers
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/Companies/Queries/`
- **Changes:** List and GetById queries
- **Validation:** Queries work

### Task 6: Create CompaniesController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/CompaniesController.cs`
- **Changes:** CRUD + user assignment endpoints
- **Validation:** Endpoints work

### Task 7: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Companies/`
- **Test Cases:** All handlers
- **Validation:** All tests pass

### Task 8: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/CompaniesEndpointTests.cs`
- **Test Cases:** Full flow
- **Validation:** All tests pass

## Documentation Tasks

### Task 9: Document Company Management
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/0011-company-management.md`
- **Changes:** Document company CRUD, user assignment, and default company
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] Organization scoping enforced
- [ ] User assignment works
- [ ] Default company works
- [ ] All unit tests pass
- [ ] All integration tests pass

