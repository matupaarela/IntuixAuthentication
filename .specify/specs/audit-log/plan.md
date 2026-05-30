# audit-log - Implementation Plan

## Architecture Design

```
AuditService (Domain Service)
  → RecordEventAsync(AuditEvent)
    → Creates AuditLog entity
    → Saves to auth_audit_logs (fire-and-forget)

AuditLogsController
  → GET /api/audit-logs → AuditLogGetListQuery
```

## Domain Changes

### 1. Create AuditLog Entity

**File:** `Domain/Entities/AuditLog.cs`

```csharp
public class AuditLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = default!;
    public string? Entity { get; set; }
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### 2. Create AuditEvent DTO

**File:** `Application/Common/DTOs/AuditEvent.cs`

```csharp
public class AuditEvent
{
    public string Action { get; set; } = default!;
    public string? Entity { get; set; }
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
}
```

### 3. Create IAuditService Interface

**File:** `Application/Common/Interfaces/IAuditService.cs`

```csharp
public interface IAuditService
{
    Task RecordAsync(AuditEvent auditEvent);
}
```

## Application Changes

### 1. Create Feature Folder

```
Application/AuditLogs/
├── Queries/
│   ├── AuditLogGetListQuery.cs
│   └── AuditLogGetListQueryHandler.cs
├── DTOs/
│   └── AuditLogResponse.cs
├── Validators/
└── Interfaces/
    └── IAuditLogRepository.cs
```

## Infrastructure Changes

### 1. Create AuditService

**File:** `Infrastructure/Security/AuditService.cs`

Implements `IAuditService`. Records events asynchronously.

### 2. Create AuditLogConfiguration

**File:** `Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs`

### 3. Add DbSet to AuthDbContext

**File:** `Infrastructure/Persistence/AuthDbContext.cs`

```csharp
public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
```

### 4. Add Query Filter

```csharp
modelBuilder.Entity<AuditLog>()
    .HasQueryFilter(a => a.TenantId == _currentUser.TenantId);
```

## API Changes

### 1. Create AuditLogsController

**File:** `Api/Controllers/AuditLogsController.cs`

GET endpoint with `AUDIT_VIEW` permission.

## Security Considerations

1. Audit logs are append-only
2. No update or delete endpoints
3. Tenant-scoped queries

## Migration Strategy

1. Create migration for `auth_audit_logs` table
2. Add index on `tenant_id`, `timestamp`

## Testing Strategy

1. Unit tests for AuditService
2. Integration tests for query endpoint
3. Verify append-only behavior

## Rollback Strategy

1. Remove AuditService and controller
2. Drop migration
