# 0009-tenant-isolation - Tasks

## Implementation Tasks

### Task 1: Audit Current Query Filters
- **Priority:** High
- **Status:** Completed
- **File:** `Infrastructure/Persistence/AuthDbContext.cs`
- **Changes:**
  - List all entities with query filters
  - Identify entities without query filters
  - Verify filter correctness
- **Validation:** Complete audit document

### Task 2: Add Missing Query Filters
- **Priority:** High
- **Status:** Completed
- **File:** `Infrastructure/Persistence/AuthDbContext.cs`
- **Changes:**
  - Add query filter for `Company` (via Organization)
  - Add query filter for `UserRole` (via User)
  - Add query filter for `UserCompany` (via User)
  - Add query filter for `RefreshToken` (via User)
  - Add query filter for `RolePermission` (via Role)
- **Validation:** All entities have query filters

### Task 3: Test Query Filter Translatability
- **Priority:** High
- **Status:** Completed
- **Changes:**
  - Test each query filter generates valid SQL
  - If navigation-based filters fail, add `TenantId` columns
  - Update entity configurations
- **Validation:** All filters translate to SQL

### Task 4: Add TenantId to Entities (If Needed)
- **Priority:** Medium
- **Status:** Cancelled
- **Files:**
  - `Domain/Entities/Company.cs` - Add `TenantId`
  - `Domain/Entities/UserRole.cs` - Add `TenantId`
  - `Domain/Entities/UserCompany.cs` - Add `TenantId`
  - `Domain/Entities/RefreshToken.cs` - Add `TenantId`
  - `Domain/Entities/RolePermission.cs` - Add `TenantId`
- **Changes:** Add property and update configurations
- **Validation:** Entities compile

### Task 5: Create Migration (If Columns Added)
- **Priority:** Medium
- **Status:** Cancelled
- **Changes:**
  - Generate migration
  - Backfill `TenantId` from parent entities
  - Make non-nullable
  - Add indexes
- **Validation:** Migration applies cleanly

### Task 6: Write Architecture Tests
- **Priority:** High
- **Status:** Completed
- **File:** `tests/Intuix.Authentication.ArchitectureTests/TenantIsolationArchitectureTests.cs`
- **Test Cases:**
  - All tenant-scoped entities have query filters
  - No `IgnoreQueryFilters()` usage without justification
  - All `TenantId` properties are indexed
- **Validation:** All tests pass

### Task 7: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/TenantIsolationTests.cs`
- **Test Cases:**
  - Create user in Tenant A
  - Query as Tenant B → empty
  - Create role in Tenant A
  - Query as Tenant B → empty
  - Create company in Tenant A
  - Query as Tenant B → empty
- **Validation:** All tests pass

### Task 8: Document Tenant Isolation Rules
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/0009-tenant-isolation.md`
- **Changes:** Document rules, entities, and how to add new tenant-scoped entities
- **Validation:** Documentation complete

## Validation Checkpoints

- [X] All entities have query filters
- [X] Filters translate to SQL
- [X] Architecture tests pass
- [ ] Integration tests pass
- [X] No cross-tenant data leakage
- [ ] Documentation complete

