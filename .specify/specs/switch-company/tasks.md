# switch-company - Tasks

## Implementation Tasks

### Task 1: Create ICompanyRepository Interface
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Auth/Interfaces/ICompanyRepository.cs`
- **Changes:** Define `GetByIdAsync(Guid id)` method
- **Validation:** Interface compiles

### Task 2: Implement CompanyRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/CompanyRepository.cs`
- **Changes:** Implement `GetByIdAsync` using `AuthDbContext`
- **Validation:** Returns company or null

### Task 3: Register ICompanyRepository in DI
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Program.cs`
- **Changes:** `builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();`
- **Validation:** Service resolves correctly

### Task 4: Enhance SwitchCompanyCommandHandler
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Auth/Commands/SwitchCompany/SwitchCompanyCommandHandler.cs`
- **Changes:**
  - Inject `ICompanyRepository`
  - Validate company exists
  - Validate company is active
  - Validate company belongs to user's tenant
  - Load full user entity
- **Validation:** All validations work

### Task 5: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Auth/Commands/SwitchCompanyCommandHandlerTests.cs`
- **Test Cases:**
  - Valid company → new JWT
  - Non-existent company → throws
  - Inactive company → throws
  - Unauthorized company → throws
  - Cross-tenant company → throws
- **Validation:** All tests pass

### Task 6: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/SwitchCompanyEndpointTests.cs`
- **Test Cases:**
  - Valid switch → 200
  - Invalid company → 400
  - Unauthenticated → 401
- **Validation:** All tests pass

## Documentation Tasks

### Task 7: Document Company Switching
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/switch-company.md`
- **Changes:** Document the company switch flow and JWT regeneration
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] Company validation works
- [ ] Tenant isolation enforced
- [ ] New JWT contains correct company
- [ ] Refresh token not rotated
- [ ] All unit tests pass
- [ ] All integration tests pass
