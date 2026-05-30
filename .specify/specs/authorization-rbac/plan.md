# authorization-rbac - Implementation Plan

## Architecture Design

The authorization system uses ASP.NET Core policy-based authorization with custom `PermissionRequirement`. The flow:

```
[Authorize(Policy = "USER_MANAGE")]
  → PermissionPolicyProvider.GetPolicyAsync("USER_MANAGE")
    → Creates AuthorizationPolicy with PermissionRequirement("USER_MANAGE")
  → PermissionAuthorizationHandler.HandleRequirementAsync()
    → Checks "perm" claims in JWT
    → If any required permission is present → context.Succeed()
```

No database queries occur during this flow.

## Domain Changes

No domain changes.

## Application Changes

### 1. Define Permission Constants

**File:** `Application/Common/Constants/Permissions.cs`

```csharp
public static class Permissions
{
    public const string TenantManage = "TENANT_MANAGE";
    public const string OrganizationManage = "ORGANIZATION_MANAGE";
    public const string CompanyManage = "COMPANY_MANAGE";
    public const string UserManage = "USER_MANAGE";
    public const string RoleManage = "ROLE_MANAGE";
    public const string PermissionManage = "PERMISSION_MANAGE";
    public const string DeviceManage = "DEVICE_MANAGE";
    public const string AuditView = "AUDIT_VIEW";
    public const string ApiKeyManage = "APIKEY_MANAGE";
    public const string MfaManage = "MFA_MANAGE";
}
```

### 2. Verify All Endpoints Have Authorization

Audit all controllers and ensure:
- `[Authorize(Policy = "...")]` on protected endpoints
- `[AllowAnonymous]` on public endpoints

## Infrastructure Changes

No infrastructure changes. The authorization system is already implemented.

### Existing Components (No Changes Needed)

1. `PermissionRequirement` - Authorization requirement
2. `PermissionPolicyProvider` - Dynamic policy creation
3. `PermissionAuthorizationHandler` - Permission checking

## API Changes

### 1. Audit All Controllers

Ensure all controllers have proper authorization:
- `AuthController`: login/refresh = AllowAnonymous, others = Authorize
- `TenantsController`: all = Policy "TENANT_MANAGE"
- `OrganizationsController`: all = Policy "ORGANIZATION_MANAGE"
- `CompaniesController`: all = Policy "COMPANY_MANAGE"
- `UsersController`: all = Policy "USER_MANAGE"
- `RolesController`: all = Policy "ROLE_MANAGE"
- `PermissionsController`: all = Policy "PERMISSION_MANAGE"

## Security Considerations

1. All endpoints must be audited for authorization
2. No endpoint may be accidentally public
3. Permission codes must be consistent

## Migration Strategy

No migration required. Seed permissions in database.

## Testing Strategy

### Architecture Tests

1. `AuthorizationArchitectureTests`:
   - All controllers have `[Authorize]` or `[AllowAnonymous]`
   - No endpoint is accessible without authorization
   - All permission policies resolve correctly

### Unit Tests

1. `PermissionAuthorizationHandlerTests`:
   - Valid permission → succeeds
   - Missing permission → fails
   - Multiple permissions (OR logic) → works

### Integration Tests

1. `AuthorizationEndpointTests`:
   - Request without token → 401
   - Request with valid token + correct permission → 200
   - Request with valid token + missing permission → 403

## Rollback Strategy

1. No rollback needed (additive changes)

## Permission Seed Data

```sql
INSERT INTO auth_permissions (code, description) VALUES
('TENANT_MANAGE', 'Gestionar tenants'),
('ORGANIZATION_MANAGE', 'Gestionar organizaciones'),
('COMPANY_MANAGE', 'Gestionar empresas'),
('USER_MANAGE', 'Gestionar usuarios'),
('ROLE_MANAGE', 'Gestionar roles'),
('PERMISSION_MANAGE', 'Gestionar permisos'),
('DEVICE_MANAGE', 'Gestionar dispositivos'),
('AUDIT_VIEW', 'Ver auditoría'),
('APIKEY_MANAGE', 'Gestionar API keys'),
('MFA_MANAGE', 'Gestionar MFA');
```
