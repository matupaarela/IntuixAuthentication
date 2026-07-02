# tenant-management - Specification

## Business Context

Tenants are the top-level organizational unit in the multi-tenant hierarchy. Each tenant represents an isolated business entity (e.g., "Intuix Holding", "Quipu Group"). Tenant management enables CRUD operations on tenant records, which is foundational for onboarding new business units.

## Problem Statement

Currently there is no API for managing tenants. The `Tenant` entity and `TenantRepository` exist but are only used internally for tenant resolution during login. An administrative API is needed to create, read, update, and list tenants.

## Goals

1. List all tenants (admin only)
2. Get tenant by ID
3. Create new tenants
4. Update tenant details (name, code, active status)
5. Enforce tenant isolation (tenants cannot see other tenants' data)

## Non-Goals

- Tenant deletion (soft-delete via `IsActive` is sufficient)
- Tenant provisioning automation
- Tenant-specific configuration

## Functional Requirements

1. **FR-01**: The system must list all tenants with pagination
2. **FR-02**: The system must get a tenant by ID
3. **FR-03**: The system must create a new tenant with name and code
4. **FR-04**: The system must update tenant name, code, and active status
5. **FR-05**: The system must validate tenant code uniqueness on create/update
6. **FR-06**: All operations require `TENANT_MANAGE` permission
7. **FR-07**: Tenant code must be alphanumeric with dashes, max 50 chars
8. **FR-08**: Tenant name must be max 150 chars

## Non-Functional Requirements

1. **NFR-01**: List operations must support pagination (max 100 per page)
2. **NFR-02**: All operations must complete within 200ms

## Acceptance Criteria

- [ ] GET /api/tenants returns paginated list
- [ ] GET /api/tenants/{id} returns tenant
- [ ] POST /api/tenants creates tenant
- [ ] PUT /api/tenants/{id} updates tenant
- [ ] Duplicate code returns 400
- [ ] Missing permission returns 403
- [ ] Invalid input returns 400 with validation errors

## Security Requirements

1. All operations require `TENANT_MANAGE` permission
2. Tenant data must not leak across tenants

## Tenant Isolation Requirements

1. Tenant management is cross-tenant by nature (admin manages all tenants)
2. Query filters should NOT apply to tenant listing (admin view)

## API Contract

### GET /api/tenants

**Query Parameters:** `page` (default 1), `pageSize` (default 20, max 100)

**Response (200):**
```json
{
  "items": [
    {
      "id": "guid",
      "name": "string",
      "code": "string",
      "isActive": true,
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ],
  "total": 10,
  "page": 1,
  "pageSize": 20
}
```

### GET /api/tenants/{id}

**Response (200):** Tenant object

### POST /api/tenants

**Request:**
```json
{
  "name": "string",
  "code": "string"
}
```

### PUT /api/tenants/{id}

**Request:**
```json
{
  "name": "string",
  "code": "string",
  "isActive": true
}
```

## Database Impact

- Reads from: `auth_tenants`
- Writes to: `auth_tenants`

## Risks

1. **Tenant creation isolation**: New tenant has no data. Mitigated by requiring seed data setup.

## Dependencies

- `ITenantRepository` - Tenant CRUD operations
- `ICurrentUser` - Permission checks
