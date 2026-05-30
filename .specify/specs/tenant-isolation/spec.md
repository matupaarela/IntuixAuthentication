# tenant-isolation - Specification

## Business Context

Tenant isolation is the foundation of multi-tenancy. Every tenant-scoped entity must be filtered by the current tenant. Cross-tenant access must be impossible through the application layer.

## Problem Statement

The current implementation applies query filters to `User`, `Role`, and `Organization` entities. However, `Company`, `UserRole`, `UserCompany`, `RefreshToken`, and `RolePermission` entities lack query filters. Additionally, there are no architecture tests to verify isolation is maintained.

## Goals

1. Apply query filters to all tenant-scoped entities
2. Create architecture tests to verify query filter coverage
3. Document tenant isolation rules
4. Ensure all new features respect tenant boundaries

## Non-Goals

- Database-level row-level security
- Cross-tenant data migration
- Tenant-specific schemas

## Functional Requirements

1. **FR-01**: All tenant-scoped entities must have EF Core query filters
2. **FR-02**: Query filters must use `ICurrentUser.TenantId`
3. **FR-03**: Entities without `TenantId` must be filtered through their parent entity
4. **FR-04**: Architecture tests must verify query filter presence
5. **FR-05**: All repository queries must respect tenant boundaries
6. **FR-06**: `SetTenant()` must only be called during authentication flows

## Non-Functional Requirements

1. **NFR-01**: Query filters must add less than 5ms to queries
2. **NFR-02**: No cross-tenant data leakage is acceptable

## Acceptance Criteria

- [ ] All tenant-scoped entities have query filters
- [ ] Architecture tests verify query filter presence
- [ ] Cross-tenant queries return empty results
- [ ] `Company` entity is filtered via `Organization.TenantId`
- [ ] `UserRole` entity is filtered via `User.TenantId`
- [ ] `UserCompany` entity is filtered via `User.TenantId`
- [ ] `RefreshToken` entity is filtered via `User.TenantId`
- [ ] `RolePermission` entity is filtered via `Role.TenantId`

## Tenant Isolation Rules

### Entities with Direct TenantId

| Entity | Query Filter |
|--------|-------------|
| User | `.HasQueryFilter(u => u.TenantId == _currentUser.TenantId)` |
| Role | `.HasQueryFilter(r => r.TenantId == _currentUser.TenantId)` |
| Organization | `.HasQueryFilter(o => o.TenantId == _currentUser.TenantId)` |

### Entities with Indirect Tenant Scoping

| Entity | Scoping Method |
|--------|---------------|
| Company | Via Organization.TenantId |
| UserRole | Via User.TenantId |
| UserCompany | Via User.TenantId |
| RefreshToken | Via User.TenantId |
| RolePermission | Via Role.TenantId |

## Database Impact

- Modify `AuthDbContext.OnModelCreating` to add missing query filters

## Risks

1. **Query filter performance**: Adding filters to more entities may impact query performance. Mitigated by proper indexing.
2. **Include chains**: Loading related entities may bypass filters. Mitigated by explicit includes.

## Dependencies

- `ICurrentUser` - Provides current tenant ID
- `AuthDbContext` - Applies query filters
