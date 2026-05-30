# authorization-rbac - Specification

## Business Context

The authorization system uses RBAC (Role-Based Access Control) with permission-based policies. Users are assigned roles, roles are assigned permissions, and permissions are checked via ASP.NET Core policy-based authorization. Permissions are embedded in JWT claims to avoid database queries during request authorization.

## Problem Statement

The current authorization system is implemented but lacks comprehensive testing, documentation, and enforcement across all endpoints. Some endpoints may not have proper policy attributes. The system needs validation that all endpoints use correct policies.

## Goals

1. Ensure all endpoints have proper authorization policies
2. Verify permission-based policies work correctly
3. Document the authorization architecture
4. Add permission seeding for all features
5. Create architecture tests to enforce authorization rules

## Non-Goals

- Dynamic permission loading
- Permission caching beyond JWT
- Cross-tenant authorization

## Functional Requirements

1. **FR-01**: All API endpoints must have `[Authorize]` or `[Authorize(Policy = "...")]`
2. **FR-02**: Public endpoints must use `[AllowAnonymous]`
3. **FR-03**: Permission policies must be checked via `PermissionRequirement`
4. **FR-04**: Permissions must be embedded in JWT claims
5. **FR-05**: Authorization must not query the database during request execution
6. **FR-06**: New features must define their permission codes
7. **FR-07**: Permission codes must follow `ENTITY_ACTION` format

## Non-Functional Requirements

1. **NFR-01**: Authorization check must complete within 10ms
2. **NFR-02**: No database queries during authorization

## Acceptance Criteria

- [ ] All endpoints have authorization attributes
- [ ] Permission policies work for all features
- [ ] Architecture tests verify authorization coverage
- [ ] Permission codes are consistent across the system
- [ ] JWT claims contain all required permissions

## Permission Codes

| Feature | Permission Code |
|---------|----------------|
| Tenant Management | `TENANT_MANAGE` |
| Organization Management | `ORGANIZATION_MANAGE` |
| Company Management | `COMPANY_MANAGE` |
| User Management | `USER_MANAGE` |
| Role Management | `ROLE_MANAGE` |
| Permission Management | `PERMISSION_MANAGE` |
| Device Management | `DEVICE_MANAGE` |
| Audit Log | `AUDIT_VIEW` |
| API Keys | `APIKEY_MANAGE` |
| MFA Management | `MFA_MANAGE` |

## Security Requirements

1. No endpoint may be accessible without authentication (except explicitly public)
2. Permission checks must be enforced at the controller level
3. No business logic authorization in handlers

## Tenant Isolation Requirements

1. Tenant claim in JWT must match the tenant being accessed
2. Cross-tenant access must be denied by query filters

## Database Impact

- No direct database impact (claims-based)

## Risks

1. **Stale permissions**: JWT permissions are set at login. Permission changes take effect on next login. Mitigated by short JWT expiry.

## Dependencies

- `PermissionRequirement`
- `PermissionPolicyProvider`
- `PermissionAuthorizationHandler`
- `ICurrentUser`
