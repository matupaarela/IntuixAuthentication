# API Contracts: Multi-Tenant Auth Hardening

## POST /auth/login

### Request

```json
{
  "username": "string",
  "password": "string",
  "tenantCode": "string"
}
```

### Success Response

```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresAt": "2026-07-02T12:00:00Z",
  "userId": "guid",
  "tenantId": "guid",
  "companyId": "guid"
}
```

### Failure Response

- Generic authentication error

## POST /auth/refresh

### Request

```json
{
  "refreshToken": "string"
}
```

`refreshToken` is required and must be non-empty.

### Success Response

```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresAt": "2026-07-02T12:00:00Z",
  "userId": "guid",
  "tenantId": "guid",
  "companyId": "guid"
}
```

### Failure Response

- Generic session error for missing, malformed, expired, revoked, or reused tokens

## POST /auth/logout

### Request

```json
{
  "refreshToken": "string"
}
```

### Success Response

```json
{
  "message": "Logged out successfully"
}
```

## POST /auth/logout-all

### Success Response

```json
{
  "message": "All sessions terminated"
}
```

## POST /auth/switch-company

### Request

```json
{
  "companyId": "guid"
}
```

### Success Response

Uses the auth response envelope with a new access token, the current user/tenant/company identifiers, and an empty refresh token field.

### Failure Response

- Generic unauthorized-company error for inactive companies, inactive organizations, cross-tenant requests, or missing membership

## GET /api/devices

### Success Response

The response contains the current user's active device sessions, including device metadata and current-session status.

```json
{
  "sessions": [
    {
      "tokenId": "guid",
      "ipAddress": "string",
      "userAgent": "string",
      "createdAt": "2026-07-02T12:00:00Z",
      "lastUsedAt": "2026-07-02T12:00:00Z",
      "isCurrent": true
    }
  ]
}
```

Legacy active rows are backfilled from `CreatedAt` during migration, so `lastUsedAt` is always present for active sessions.

## DELETE /api/devices/{tokenId}

### Success Response

```json
{
  "message": "Session revoked"
}
```

## POST /api/devices/revoke-all

### Success Response

```json
{
  "message": "All other sessions revoked"
}
```

## Shared Error Contract

- Generic authentication error
- Generic session error
- Generic authorization error
- Generic internal error without secrets or token values
- Detailed reasons remain internal and are not returned to clients
