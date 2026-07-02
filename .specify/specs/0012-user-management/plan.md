# 0012-user-management - Implementation Plan

## Architecture Design

```
UsersController
  → GET /api/users → UserGetListQuery
  → GET /api/users/{id} → UserGetByIdQuery
  → POST /api/users → UserCreateCommand
  → PUT /api/users/{id} → UserUpdateCommand
  → PUT /api/users/{id}/password → UserResetPasswordCommand
  → PUT /api/users/{id}/lock → UserLockCommand
  → PUT /api/users/{id}/unlock → UserUnlockCommand
  → POST /api/users/{id}/roles → UserRoleAssignCommand
  → DELETE /api/users/{id}/roles/{roleId} → UserRoleRemoveCommand
  → GET /api/users/{id}/roles → UserGetRolesQuery
  → GET /api/users/{id}/companies → UserGetCompaniesQuery
```

## Domain Changes

No domain changes.

## Application Changes

### 1. Create Feature Folder

```
Application/Users/
├── Commands/
│   ├── UserCreateCommand.cs/.Handler.cs
│   ├── UserUpdateCommand.cs/.Handler.cs
│   ├── UserResetPasswordCommand.cs/.Handler.cs
│   ├── UserLockCommand.cs/.Handler.cs
│   ├── UserUnlockCommand.cs/.Handler.cs
│   ├── UserRoleAssignCommand.cs/.Handler.cs
│   └── UserRoleRemoveCommand.cs/.Handler.cs
├── Queries/
│   ├── UserGetListQuery.cs/.Handler.cs
│   ├── UserGetByIdQuery.cs/.Handler.cs
│   ├── UserGetRolesQuery.cs/.Handler.cs
│   └── UserGetCompaniesQuery.cs/.Handler.cs
├── DTOs/
│   ├── UserResponse.cs
│   ├── UserCreateRequest.cs
│   └── UserRoleResponse.cs
├── Validators/
└── Interfaces/
    └── IUserRepository.cs (extend existing)
```

### 2. Extend IUserRepository

Add methods:
- `GetAllByTenantAsync(Guid tenantId, string? search, int page, int pageSize)`
- `GetCountByTenantAsync(Guid tenantId, string? search)`
- `GetByIdAsync(Guid id)`
- `CreateAsync(User user)`
- `UpdateAsync(User user)`
- `UpdatePasswordAsync(Guid userId, byte[] passwordHash)`
- `LockAsync(Guid userId)`
- `UnlockAsync(Guid userId)`
- `AssignRoleAsync(Guid userId, Guid roleId)`
- `RemoveRoleAsync(Guid userId, Guid roleId)`
- `GetUserRolesAsync(Guid userId)`
- `GetUserCompaniesAsync(Guid userId)`

## Infrastructure Changes

### 1. Extend UserRepository

Implement new methods.

## API Changes

### 1. Create UsersController

CRUD + role assignment + password management endpoints.

## Security Considerations

1. Password hash never returned in responses
2. Password reset uses `IPasswordHasher`
3. All operations require `USER_MANAGE` permission

## Migration Strategy

No migration required.

## Testing Strategy

1. Unit tests for all handlers
2. Integration tests for endpoints
3. Verify password hashing
4. Verify tenant scoping

## Rollback Strategy

1. Remove new files
2. No database changes

