# 0008-tenant-management - Implementation Plan

## Architecture Design

```
TenantsController
  → GET /api/tenants → TenantGetListQuery → List<TenantResponse>
  → GET /api/tenants/{id} → TenantGetByIdQuery → TenantResponse
  → POST /api/tenants → TenantCreateCommand → TenantResponse
  → PUT /api/tenants/{id} → TenantUpdateCommand → TenantResponse
```

## Domain Changes

No domain changes. `Tenant` entity already has all required fields.

## Application Changes

### 1. Create Feature Folder

```
Application/Tenants/
├── Commands/
│   ├── TenantCreateCommand.cs
│   ├── TenantCreateCommandHandler.cs
│   ├── TenantUpdateCommand.cs
│   └── TenantUpdateCommandHandler.cs
├── Queries/
│   ├── TenantGetListQuery.cs
│   ├── TenantGetListQueryHandler.cs
│   ├── TenantGetByIdQuery.cs
│   └── TenantGetByIdQueryHandler.cs
├── DTOs/
│   ├── TenantResponse.cs
│   ├── TenantCreateRequest.cs
│   └── TenantUpdateRequest.cs
├── Validators/
│   ├── TenantCreateCommandValidator.cs
│   └── TenantUpdateCommandValidator.cs
└── Interfaces/
    └── ITenantRepository.cs (extend existing)
```

### 2. Extend ITenantRepository

Add methods:
- `Task<List<Tenant>> GetAllAsync(int page, int pageSize)`
- `Task<int> GetCountAsync()`
- `Task<Tenant?> GetByIdAsync(Guid id)`
- `Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)`
- Task AddAsync(Tenant tenant)
- Task UpdateAsync(Tenant tenant)

## Infrastructure Changes

### 1. Extend TenantRepository

Implement new methods.

## API Changes

### 1. Create TenantsController

**File:** `Api/Controllers/TenantsController.cs`

Endpoints:
- `GET /api/tenants` → `[Authorize(Policy = "TENANT_MANAGE")]`
- `GET /api/tenants/{id}` → `[Authorize(Policy = "TENANT_MANAGE")]`
- `POST /api/tenants` → `[Authorize(Policy = "TENANT_MANAGE")]`
- `PUT /api/tenants/{id}` → `[Authorize(Policy = "TENANT_MANAGE")]`

## Security Considerations

1. All endpoints require `TENANT_MANAGE` permission
2. Tenant code uniqueness must be validated

## Migration Strategy

No migration required.

## Testing Strategy

### Unit Tests

1. `TenantCreateCommandHandlerTests`:
   - Valid input → tenant created
   - Duplicate code → throws
   - Invalid code format → throws

2. `TenantUpdateCommandHandlerTests`:
   - Valid input → tenant updated
   - Duplicate code → throws
   - Non-existent → throws

### Integration Tests

1. `TenantsEndpointTests`:
   - CRUD operations work
   - Permission checks work
   - Validation works

## Rollback Strategy

1. Remove new controller and feature files
2. No database changes

