# 0006-auth-api-keys - Specification

## Business Context

Machine-to-machine communication requires API keys for authentication. API keys provide a simple authentication mechanism for services, scripts, and integrations that cannot use interactive authentication flows.

## Problem Statement

No API key management exists. Services currently share user credentials or have no authentication. API keys provide a secure, revocable, and auditable mechanism for machine authentication.

## Goals

1. Generate API keys for services/integrations
2. API keys are scoped to a tenant
3. API keys have configurable expiry
4. API keys can be revoked
5. API key usage is audited

## Non-Goals

- OAuth client credentials flow (separate feature)
- API key rate limiting
- API key scoping to specific endpoints

## Functional Requirements

1. **FR-01**: Generate API key with name and optional expiry
2. **FR-02**: API key is returned once (raw value), only hash stored
3. **FR-03**: List all API keys (showing name, last used, expiry)
4. **FR-04**: Revoke API key by ID
5. **FR-05**: API keys are tenant-scoped
6. **FR-06**: Require `APIKEY_MANAGE` permission
7. **FR-07**: API key authentication produces a JWT with limited claims

## Non-Functional Requirements

1. **NFR-01**: API key must be cryptographically random (64 bytes)
2. **NFR-02**: API key must be stored hashed (SHA256)

## Acceptance Criteria

- [ ] POST /api/0006-auth-api-keys generates new key (returned once)
- [ ] GET /api/0006-auth-api-keys lists all keys (no raw values)
- [ ] DELETE /api/0006-auth-api-keys/{id} revokes key
- [ ] API key hash stored, raw value returned once
- [ ] Expired keys rejected
- [ ] Revoked keys rejected

## API Contract

### POST /api/0006-auth-api-keys

**Request:**
```json
{
  "name": "string",
  "expiresAt": "2026-12-31T00:00:00Z (optional)"
}
```

**Response (200):**
```json
{
  "id": "guid",
  "name": "string",
  "key": "string (raw, shown once)",
  "expiresAt": "2026-12-31T00:00:00Z",
  "createdAt": "2026-05-29T10:00:00Z"
}
```

### GET /api/0006-auth-api-keys

**Response (200):**
```json
{
  "items": [
    {
      "id": "guid",
      "name": "string",
      "lastUsedAt": "2026-05-29T10:00:00Z",
      "expiresAt": "2026-12-31T00:00:00Z",
      "createdAt": "2026-05-29T10:00:00Z"
    }
  ]
}
```

### DELETE /api/0006-auth-api-keys/{id}

**Response (200):**
```json
{
  "message": "API key revoked"
}
```

## Database Impact

### New Table: `auth_api_keys`

```sql
CREATE TABLE auth_api_keys (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(100) NOT NULL,
    key_hash VARBINARY(512) NOT NULL,
    expires_at DATETIME2 NULL,
    last_used_at DATETIME2 NULL,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    FOREIGN KEY (tenant_id) REFERENCES auth_tenants(id)
);
```

## Risks

1. **Key exposure**: Raw key shown once. User must save it immediately.
2. **Key reuse**: API keys don't rotate automatically.

## Dependencies

- `ICurrentUser` - Tenant context
- `AuthDbContext` - Database

