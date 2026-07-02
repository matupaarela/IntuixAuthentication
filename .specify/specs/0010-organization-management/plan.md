# 0010-organization-management - Implementation Plan

## Architecture Design

```
OrganizationsController
  → GET /api/organizations → OrganizationGetListQuery
  → GET /api/organizations/{id} → OrganizationGetByIdQuery
  → POST /api/organizations → OrganizationCreateCommand
  → PUT /api/organizations/{id} → OrganizationUpdateCommand
```

## Domain Changes

No domain changes. `Organization` entity has `TenantId`, `Name`, `IsActive`.

## Application Changes

### 1. Create Feature Folder

```
Application/Organizations/
├── Commands/
│   ├── OrganizationCreateCommand.cs
│   ├── OrganizationCreateCommandHandler.cs
│   ├── OrganizationUpdateCommand.cs
│   └── OrganizationUpdateCommandHandler.cs
├── Queries/
│   ├── OrganizationGetListQuery.cs
│   ├── OrganizationGetListQueryHandler.cs
│   ├── OrganizationGetByIdQuery.cs
│   └── OrganizationGetByIdQueryHandler.cs
├── DTOs/
│   ├── OrganizationResponse.cs
│   ├── OrganizationCreateRequest.cs
│   └── OrganizationUpdateRequest.cs
├── Validators/
│   └── OrganizationCreateCommandValidator.cs
└── Interfaces/
    └── IOrganizationRepository.cs
```

### 2. Create IOrganizationRepository

```csharp
public interface IOrganizationRepository
{
    Task<List<Organization>> GetAllByTenantAsync(Guid tenantId, int page, int pageSize);
    Task<int> GetCountByTenantAsync(Guid tenantId);
    Task<Organization?> GetByIdAsync(Guid id);
    Task AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
}
```

## Infrastructure Changes

### 1. Implement OrganizationRepository

**File:** `Infrastructure/Persistence/Repositories/OrganizationRepository.cs`

All queries scoped to tenant via `TenantId`.

## API Changes

### 1. Create OrganizationsController

All endpoints with `[Authorize(Policy = "ORGANIZATION_MANAGE")]`.

## Security Considerations

1. All queries scoped to tenant
2. Permission checks on all endpoints

## Migration Strategy

No migration required.

## Testing Strategy

### Unit Tests

1. Organization CRUD handlers
2. Tenant scoping validation

### Integration Tests

1. Full CRUD flow
2. Cross-tenant isolation

## Rollback Strategy

1. Remove new files
2. No database changes

