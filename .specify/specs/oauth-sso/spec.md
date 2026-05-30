# oauth-sso - Specification

## Business Context

Organizations use identity providers (Google, Microsoft, corporate SAML) for single sign-on. OAuth/SSO integration allows users to authenticate using their existing corporate credentials, reducing password fatigue and centralizing identity management.

## Problem Statement

No external identity provider integration exists. All users authenticate with local credentials. OAuth/SSO enables federation with external identity providers using OAuth 2.0 / OpenID Connect.

## Goals

1. Authenticate users via external OAuth 2.0 providers (Google, Microsoft)
2. Link external accounts to local users
3. Auto-provision users on first SSO login
4. Support multiple providers per tenant
5. Store provider configuration per tenant

## Non-Goals

- SAML integration (future)
- LDAP integration
- Custom OAuth providers
- Just-In-Time provisioning with role assignment
- Provider-specific attribute mapping

## Functional Requirements

1. **FR-01**: System must support OAuth 2.0 Authorization Code flow
2. **FR-02**: System must support Google and Microsoft providers
3. **FR-03**: Provider configuration stored per tenant
4. **FR-04**: On first SSO login, user is auto-provisioned
5. **FR-05**: External account is linked to local user via email
6. **FR-06**: Subsequent SSO logins use linked account
7. **FR-07**: Users can link/unlink external accounts
8. **FR-08**: SSO login issues standard JWT + refresh token

## Non-Functional Requirements

1. **NFR-01**: OAuth flow must complete within 5 seconds
2. **NFR-02**: Provider tokens must not be stored locally
3. **NFR-03**: State parameter must be validated (CSRF protection)

## Acceptance Criteria

- [ ] GET /auth/sso/{provider} returns redirect URL
- [ ] POST /auth/sso/{provider}/callback exchanges code for tokens
- [ ] First SSO login creates user automatically
- [ ] Subsequent SSO logins use linked account
- [ ] Invalid state parameter rejected
- [ ] Invalid authorization code rejected
- [ ] SSO user can link additional providers
- [ ] SSO user can unlink providers

## API Contract

### GET /auth/sso/{provider}

**Response (200):**
```json
{
  "redirectUrl": "https://accounts.google.com/o/oauth2/auth?...",
  "state": "string"
}
```

### POST /auth/sso/{provider}/callback

**Request:**
```json
{
  "code": "string",
  "state": "string"
}
```

**Response (200):**
```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresAt": "2026-05-29T23:30:00Z",
  "userId": "guid",
  "tenantId": "guid",
  "companyId": "guid",
  "isNewUser": false
}
```

## Database Impact

### New Table: `auth_external_logins`

```sql
CREATE TABLE auth_external_logins (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER NOT NULL,
    provider NVARCHAR(50) NOT NULL,
    provider_user_id NVARCHAR(200) NOT NULL,
    email NVARCHAR(150) NOT NULL,
    display_name NVARCHAR(200) NULL,
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    FOREIGN KEY (user_id) REFERENCES auth_users(id),
    CONSTRAINT UQ_external_login UNIQUE (provider, provider_user_id)
);
```

### New Table: `auth_sso_providers`

```sql
CREATE TABLE auth_sso_providers (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    provider NVARCHAR(50) NOT NULL,
    client_id NVARCHAR(200) NOT NULL,
    client_secret NVARCHAR(500) NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,

    FOREIGN KEY (tenant_id) REFERENCES auth_tenants(id),
    CONSTRAINT UQ_sso_provider UNIQUE (tenant_id, provider)
);
```

## Risks

1. **Provider downtime**: External provider unavailability blocks login. Mitigated by local fallback.
2. **Email matching**: Multiple users with same email across tenants. Mitigated by tenant-scoping.

## Dependencies

- `ICurrentUser` - Tenant context
- `IUserRepository` - User creation
- `AuthDbContext` - Database
