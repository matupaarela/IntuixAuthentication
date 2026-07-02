# 0014-permission-management - Implementation Plan

## Architecture Design

```
PermissionsController
  → GET /api/permissions → PermissionGetListQuery
  → GET /api/permissions/{id} → PermissionGetByIdQuery
  → POST /api/permissions → PermissionCreateCommand
  → PUT /api/permissions/{id} → PermissionUpdateCommand
```

## Domain Changes

No domain changes.

## Application Changes

### 1. Create Feature Folder

```
Application/Permissions/
├── Commands/
│   ├── PermissionCreateCommand.cs/.Handler.cs
│   └── PermissionUpdateCommand.cs/.Handler.cs
├── Queries/
│   ├── PermissionGetListQuery.cs/.Handler.cs
│   └── PermissionGetByIdQuery.cs/.Handler.cs
├── DTOs/
│   ├── PermissionResponse.cs
│   └── PermissionCreateRequest.cs
├── Validators/
│   └── PermissionCreateCommandValidator.cs
└── Interfaces/
    └── IPermissionRepository.cs
```

### 2. Create IPermissionRepository

```csharp
public interface IPermissionRepository
{
    Task<List<Permission>> GetAllAsync(int page, int pageSize);
    Task<int> GetCountAsync();
    Task<Permission?> GetByIdAsync(Guid id);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    Task AddAsync(Permission permission);
    Task UpdateAsync(Permission permission);
}
```

## Infrastructure Changes

### 1. Implement PermissionRepository

## API Changes

### 1. Create PermissionsController

CRUD endpoints with `PERMISSION_MANAGE` permission.

## Security Considerations

1. Permissions are global (not tenant-scoped)
2. Permission checks on all endpoints

## Migration Strategy

No migration required.

## Testing Strategy

1. Unit tests for handlers
2. Integration tests for endpoints

## Rollback Strategy

1. Remove new files
2. No database changes

