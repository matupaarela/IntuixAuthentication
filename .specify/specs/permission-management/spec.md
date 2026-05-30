# permission-management - Specification

## Business Context

Permissions are the atomic units of authorization. They are assigned to roles and embedded in JWT claims. Permission management enables CRUD operations on the permission catalog.

## Problem Statement

No API exists for managing permissions. The `Permission` entity exists but is only used for JWT claims. An administrative API is needed for the permission catalog.

## Goals

1. CRUD operations for permissions (global, not tenant-scoped)
2. List all permissions
3. View permission details
4. Create new permission codes
5. Update permission descriptions

## Non-Goals

- Permission hierarchy
- Permission groups
- Dynamic permissions

## Functional Requirements

1. **FR-01**: List all permissions with pagination
2. **FR-02**: Get permission by ID
3. **FR-03**: Create permission with code and description
4. **FR-04**: Update permission description
5. **FR-05**: Validate code uniqueness
6. **FR-06**: Require `PERMISSION_MANAGE` permission

## Acceptance Criteria

- [ ] GET /api/permissions returns paginated list
- [ ] GET /api/permissions/{id} returns permission
- [ ] POST /api/permissions creates permission
- [ ] PUT /api/permissions/{id} updates permission
- [ ] Duplicate code returns 400
- [ ] Code format validated (UPPER_SNAKE_CASE)

## API Contract

### GET /api/permissions

**Response (200):**
```json
{
  "items": [
    {
      "id": "guid",
      "code": "USER_CREATE",
      "description": "Crear usuarios"
    }
  ],
  "total": 10,
  "page": 1,
  "pageSize": 20
}
```

### POST /api/permissions

**Request:**
```json
{
  "code": "USER_CREATE",
  "description": "Crear usuarios"
}
```

## Database Impact

- Reads/writes to: `auth_permissions`

## Dependencies

- `IPermissionRepository`
- `ICurrentUser`
