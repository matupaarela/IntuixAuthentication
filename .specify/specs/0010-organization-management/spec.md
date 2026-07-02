# 0010-organization-management - Specification

## Business Context

Organizations exist within tenants and group companies. They represent business divisions or subsidiaries. Organization management enables CRUD operations scoped to the tenant.

## Problem Statement

No API exists for managing organizations. The `Organization` entity exists but is only used for company grouping. An administrative API is needed.

## Goals

1. CRUD operations for organizations
2. Tenant-scoped isolation
3. List organizations within tenant
4. Validate organization belongs to tenant

## Non-Goals

- Cross-tenant organization management
- Organization hierarchy beyond tenant

## Functional Requirements

1. **FR-01**: List organizations within current tenant
2. **FR-02**: Get organization by ID (within tenant)
3. **FR-03**: Create organization within current tenant
4. **FR-04**: Update organization name and active status
5. **FR-05**: All operations scoped to tenant
6. **FR-06**: Require `ORGANIZATION_MANAGE` permission

## Acceptance Criteria

- [ ] GET /api/organizations returns tenant-scoped list
- [ ] POST /api/organizations creates in current tenant
- [ ] PUT /api/organizations/{id} updates within tenant
- [ ] Cross-tenant access returns 404
- [ ] Permission checks work

## API Contract

### GET /api/organizations

**Response (200):**
```json
{
  "items": [
    {
      "id": "guid",
      "name": "string",
      "isActive": true,
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ],
  "total": 5,
  "page": 1,
  "pageSize": 20
}
```

### POST /api/organizations

**Request:**
```json
{
  "name": "string"
}
```

### PUT /api/organizations/{id}

**Request:**
```json
{
  "name": "string",
  "isActive": true
}
```

## Database Impact

- Reads/writes to: `auth_organizations`

## Dependencies

- `IOrganizationRepository`
- `ICurrentUser`

