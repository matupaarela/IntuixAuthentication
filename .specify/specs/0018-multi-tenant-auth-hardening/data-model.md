# Data Model: Multi-Tenant Auth Hardening

## Core Entities

### Tenant

- `Id`
- `Code` (unique)
- `Name`
- `IsActive`
- `CreatedAt`

### Organization

- `Id`
- `TenantId`
- `Name`
- `IsActive`
- `CreatedAt`

### Company

- `Id`
- `OrganizationId`
- `Name`
- `Ruc` (optional)
- `IsActive`

### User

- `Id`
- `TenantId`
- `Username` (unique per tenant)
- `Email`
- `PasswordHash`
- `IsActive`
- `IsLocked`
- `FailedAttempts`
- `LastLogin` (optional)
- `CreatedAt`

### UserCompany

- `UserId`
- `CompanyId`
- `IsDefault`

### Role

- `Id`
- `TenantId`
- `Name` (unique per tenant)

### Permission

- `Id`
- `Code` (unique)
- `Description` (optional)

### UserRole

- `UserId`
- `RoleId`

### RolePermission

- `RoleId`
- `PermissionId`

### RefreshToken

- `Id`
- `UserId`
- `TokenHash`
- `ExpiresAt`
- `CreatedAt`
- `RevokedAt` (optional)
- `ReplacedByToken` (optional)
- `Device` (optional)
- `IpAddress` (optional)
- `UserAgent` (optional)
- `RevocationReason` (optional)
- `LastUsedAt` (required)

## Relationships

- One tenant owns many organizations.
- One organization owns many companies.
- One tenant owns many users and roles.
- One user can belong to many companies through `UserCompany`.
- One user can have many roles through `UserRole`.
- One role can grant many permissions through `RolePermission`.
- One user can have many refresh tokens; each token belongs to exactly one user.

## Validation Rules

- Tenant code must be unique.
- Usernames must be unique within a tenant.
- Role names must be unique within a tenant.
- Company selection must belong to the current tenant and to the current user.
- A default company must be one of the user's assigned companies.
- A refresh token must always be associated with one user.
- A revoked token must not be accepted for renewal.
- Existing active refresh-token rows must be backfilled from `CreatedAt` so `LastUsedAt` is never null for active sessions.

## State Transitions

### User lockout

- `Active` -> `Locked` after the configured failure threshold.
- `Locked` -> `Active` only through manual support/operations release.

### Refresh session

- `Active` -> `Rotated` when a refresh token is replaced by a newer token.
- `Active` -> `Revoked` on logout, logout-all, or reuse detection.
- `Active` -> `Expired` when the expiration time passes.

## Indexing Notes

- `Tenant.Code` should remain indexed and unique.
- `(TenantId, Username)` should remain indexed and unique.
- `UserId` on refresh tokens should remain indexed.
- Active-session queries should be able to filter by `UserId`, `RevokedAt`, `LastUsedAt`, and `CreatedAt` efficiently.
