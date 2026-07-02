# 0013-role-management - Specification

## Business Context

Roles group permissions and are assigned to users. They enable RBAC (Role-Based Access Control). Role management enables CRUD operations and permission assignment within tenants.

## Problem Statement

No API exists for managing roles. The `Role` entity exists but is only used for JWT claims. An administrative API is needed.

## Goals

1. CRUD operations for roles (tenant-scoped)
2. Assign permissions to roles
3. Remove permissions from roles
4. View role permissions

## Non-Goals

- System-defined roles
- Role hierarchy
- Role templates

## Functional Requirements

1. **FR-01**: List roles within tenant
2. **FR-02**: Get role by ID with permissions
3. **FR-03**: Create role with name
4. **FR-04**: Update role name
5. **FR-05**: Assign permission to role
6. **FR-06**: Remove permission from role
7. **FR-07**: View role permissions
8. **FR-08**: Require `ROLE_MANAGE` permission

## Acceptance Criteria

- [ ] GET /api/roles returns tenant-scoped list
- [ ] GET /api/roles/{id} returns role with permissions
- [ ] POST /api/roles creates role
- [ ] PUT /api/roles/{id} updates role
- [ ] POST /api/roles/{id}/permissions assigns permission
- [ ] DELETE /api/roles/{id}/permissions/{permissionId} removes permission
- [ ] Duplicate name within tenant returns 400

## API Contract

### GET /api/roles

**Response (200):**
```json
{
  "items": [
    {
      "id": "guid",
      "name": "string",
      "permissionCount": 5
    }
  ],
  "total": 3,
  "page": 1,
  "pageSize": 20
}
```

### GET /api/roles/{id}

**Response (200):**
```json
{
  "id": "guid",
  "name": "string",
  "permissions": [
    {
      "id": "guid",
      "code": "USER_CREATE",
      "description": "Crear usuarios"
    }
  ]
}
```

### POST /api/roles

**Request:**
```json
{
  "name": "string"
}
```

### POST /api/roles/{id}/permissions

**Request:**
```json
{
  "permissionId": "guid"
}
```

## Database Impact

- Reads/writes to: `auth_roles`, `auth_role_permissions`

## Dependencies

- `IRoleRepository`
- `IPermissionRepository`
- `ICurrentUser`

