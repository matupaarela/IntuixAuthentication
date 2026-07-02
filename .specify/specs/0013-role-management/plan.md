# 0013-role-management - Implementation Plan

## Architecture Design

```
RolesController
  → GET /api/roles → RoleGetListQuery
  → GET /api/roles/{id} → RoleGetByIdQuery
  → POST /api/roles → RoleCreateCommand
  → PUT /api/roles/{id} → RoleUpdateCommand
  → POST /api/roles/{id}/permissions → RoleAssignPermissionCommand
  → DELETE /api/roles/{id}/permissions/{permissionId} → RoleRemovePermissionCommand
```

## Domain Changes

No domain changes.

## Application Changes

### 1. Create Feature Folder

```
Application/Roles/
├── Commands/
│   ├── RoleCreateCommand.cs/.Handler.cs
│   ├── RoleUpdateCommand.cs/.Handler.cs
│   ├── RoleAssignPermissionCommand.cs/.Handler.cs
│   └── RoleRemovePermissionCommand.cs/.Handler.cs
├── Queries/
│   ├── RoleGetListQuery.cs/.Handler.cs
│   └── RoleGetByIdQuery.cs/.Handler.cs
├── DTOs/
│   ├── RoleResponse.cs
│   ├── RoleDetailResponse.cs
│   └── RoleCreateRequest.cs
├── Validators/
└── Interfaces/
    └── IRoleRepository.cs
```

### 2. Create IRoleRepository

```csharp
public interface IRoleRepository
{
    Task<List<Role>> GetAllByTenantAsync(Guid tenantId, int page, int pageSize);
    Task<int> GetCountByTenantAsync(Guid tenantId);
    Task<Role?> GetByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null);
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task AssignPermissionAsync(Guid roleId, Guid permissionId);
    Task RemovePermissionAsync(Guid roleId, Guid permissionId);
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId);
}
```

## Infrastructure Changes

### 1. Implement RoleRepository

All queries scoped to tenant.

## API Changes

### 1. Create RolesController

CRUD + permission assignment endpoints with `ROLE_MANAGE` permission.

## Security Considerations

1. Tenant scoping enforced
2. Permission checks on all endpoints

## Migration Strategy

No migration required.

## Testing Strategy

1. Unit tests for all handlers
2. Integration tests for endpoints

## Rollback Strategy

1. Remove new files
2. No database changes

