# audit-log - Tasks

## Implementation Tasks

### Task 1: Create AuditLog Entity
- **Priority:** High
- **Status:** Pending
- **File:** `Domain/Entities/AuditLog.cs`
- **Changes:** Define entity with all fields
- **Validation:** Entity compiles

### Task 2: Create AuditEvent DTO
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Common/DTOs/AuditEvent.cs`
- **Changes:** Define event DTO
- **Validation:** DTO compiles

### Task 3: Create IAuditService Interface
- **Priority:** High
- **Status:** Pending
- **File:** `Application/Common/Interfaces/IAuditService.cs`
- **Changes:** Define RecordAsync method
- **Validation:** Interface compiles

### Task 4: Create AuditService Implementation
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Security/AuditService.cs`
- **Changes:**
  - Inject `AuthDbContext`, `ICurrentUser`, `IHttpContextAccessor`
  - Record events asynchronously
  - Capture IP and User-Agent
- **Validation:** Service works

### Task 5: Create AuditLogConfiguration
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs`
- **Changes:** Configure table name, indexes
- **Validation:** Configuration compiles

### Task 6: Add AuditLog to AuthDbContext
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/AuthDbContext.cs`
- **Changes:** Add DbSet and query filter
- **Validation:** DbContext compiles

### Task 7: Create Migration
- **Priority:** High
- **Status:** Pending
- **Changes:** Generate migration for auth_audit_logs
- **Validation:** Migration applies cleanly

### Task 8: Create IAuditLogRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Application/AuditLogs/Interfaces/IAuditLogRepository.cs`
- **Changes:** Query methods with filters
- **Validation:** Interface compiles

### Task 9: Implement AuditLogRepository
- **Priority:** High
- **Status:** Pending
- **File:** `Infrastructure/Persistence/Repositories/AuditLogRepository.cs`
- **Changes:** Implement query methods
- **Validation:** Methods work

### Task 10: Create AuditLogGetListQuery
- **Priority:** High
- **Status:** Pending
- **Files:** `Application/AuditLogs/Queries/`
- **Changes:** Paginated query with filters
- **Validation:** Query works

### Task 11: Create AuditLogsController
- **Priority:** High
- **Status:** Pending
- **File:** `Api/Controllers/AuditLogsController.cs`
- **Changes:** GET endpoint with `AUDIT_VIEW` permission
- **Validation:** Endpoint works

### Task 12: Integrate AuditService into Handlers
- **Priority:** High
- **Status:** Pending
- **Files:** All command handlers
- **Changes:** Inject IAuditService and record events
- **Validation:** Events recorded

### Task 13: Write Unit Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/AuditLogs/`
- **Test Cases:** AuditService, query handler
- **Validation:** All tests pass

### Task 14: Write Integration Tests
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/AuditLogsEndpointTests.cs`
- **Test Cases:** Query endpoint
- **Validation:** All tests pass

## Documentation Tasks

### Task 15: Document Audit Log
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/audit-log.md`
- **Changes:** Document audit event types, querying, and retention
- **Validation:** Documentation is complete

## Validation Checkpoints

- [ ] AuditLog entity created
- [ ] AuditService records events
- [ ] Events are tenant-scoped
- [ ] Query endpoint works
- [ ] Append-only enforced
- [ ] All unit tests pass
- [ ] All integration tests pass
