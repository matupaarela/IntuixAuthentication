# 0012-user-management - Specification

## Business Context

Users are the primary actors in the system. User management enables CRUD operations, password management, role assignment, and account status control. All operations are tenant-scoped.

## Problem Statement

No API exists for managing users beyond login. The `User` entity exists but is only used for authentication. An administrative API is needed for user lifecycle management.

## Goals

1. CRUD operations for users (tenant-scoped)
2. Password management (reset, change)
3. Account status control (lock, unlock, activate, deactivate)
4. Role assignment to users
5. View user's companies and roles

## Non-Goals

- Self-registration (separate concern)
- Profile management by user
- User deletion (soft-delete via IsActive)

## Functional Requirements

1. **FR-01**: List users within tenant with pagination
2. **FR-02**: Get user by ID (within tenant)
3. **FR-03**: Create user with username, email, password
4. **FR-04**: Update user email
5. **FR-05**: Reset user password (admin action)
6. **FR-06**: Lock/unlock user account
7. **FR-07**: Activate/deactivate user
8. **FR-08**: Assign role to user
9. **FR-09**: Remove role from user
10. **FR-10**: View user's roles
11. **FR-11**: View user's companies
12. **FR-12**: Require `USER_MANAGE` permission

## Acceptance Criteria

- [ ] GET /api/users returns tenant-scoped paginated list
- [ ] GET /api/users/{id} returns user (no password hash)
- [ ] POST /api/users creates user with hashed password
- [ ] PUT /api/users/{id} updates user
- [ ] PUT /api/users/{id}/password resets password
- [ ] PUT /api/users/{id}/lock locks account
- [ ] PUT /api/users/{id}/unlock unlocks account
- [ ] POST /api/users/{id}/roles assigns role
- [ ] DELETE /api/users/{id}/roles/{roleId} removes role
- [ ] GET /api/users/{id}/roles returns user roles
- [ ] GET /api/users/{id}/companies returns user companies

## Security Requirements

1. Password must be hashed with `IPasswordHasher`
2. Password hash must never be returned in responses
3. All operations require `USER_MANAGE` permission

## Tenant Isolation Requirements

1. All queries scoped to current tenant
2. Cannot create user in different tenant
3. Cannot assign cross-tenant roles

## API Contract

### GET /api/users

**Query:** `page`, `pageSize`, `search` (optional, filters by username/email)

### POST /api/users

**Request:**
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

### PUT /api/users/{id}/password

**Request:**
```json
{
  "newPassword": "string"
}
```

### POST /api/users/{id}/roles

**Request:**
```json
{
  "roleId": "guid"
}
```

## Database Impact

- Reads/writes to: `auth_users`, `auth_user_roles`, `auth_user_companies`

## Dependencies

- `IUserRepository`
- `IRoleRepository`
- `IPasswordHasher`
- `ICurrentUser`

