# authentication-login - Specification

## Business Context

Users must authenticate against the Intuix ecosystem using credentials (username + password) scoped to a specific tenant. The system must validate credentials, enforce account lockout policies, issue JWT access tokens with embedded permissions, and generate refresh tokens for session continuity.

## Problem Statement

The current login implementation uses plain text password comparison (`request.Password != user.PasswordHash`), lacks proper validation, and does not use the `IPasswordHasher` interface. The handler must be refactored to use secure password hashing and follow the architectural constitution.

## Goals

1. Authenticate users with secure password hashing (PBKDF2 + SHA256)
2. Issue JWT access tokens with all required claims (sub, tenant, company, role, perm, jti)
3. Generate and store hashed refresh tokens
4. Enforce account lockout after 5 failed attempts
5. Resolve tenant context before user lookup
6. Return user's default company in the response

## Non-Goals

- Multi-factor authentication (separate feature: mfa)
- OAuth/SSO login (separate feature: oauth-sso)
- Social login integration
- CAPTCHA integration

## Functional Requirements

1. **FR-01**: The system must accept `username`, `password`, and `tenantCode` as login inputs
2. **FR-02**: The system must resolve the tenant by `tenantCode` before user lookup
3. **FR-03**: The system must set the tenant context via `ICurrentUser.SetTenant()` before querying
4. **FR-04**: The system must look up the user by `username` within the resolved tenant
5. **FR-05**: The system must reject login if user is not found or inactive
6. **FR-06**: The system must reject login if user is locked
7. **FR-07**: The system must verify password using `IPasswordHasher.Verify()`
8. **FR-08**: The system must increment `FailedAttempts` on failed password verification
9. **FR-09**: The system must lock the account (`IsLocked = true`) after 5 consecutive failed attempts
10. **FR-10**: The system must reset `FailedAttempts` to 0 on successful login
11. **FR-11**: The system must set `LastLogin` to UTC now on successful login
12. **FR-12**: The system must retrieve the user's default company
13. **FR-13**: The system must retrieve user roles and permissions
14. **FR-14**: The system must generate a JWT access token via `IJwtProvider.GenerateToken()`
15. **FR-15**: The system must generate a refresh token via `IRefreshTokenService.Generate()`
16. **FR-16**: The system must store the refresh token hash in `auth_refresh_tokens`
17. **FR-17**: The system must return `AuthResponse` with access token, refresh token, expiry, userId, tenantId, companyId

## Non-Functional Requirements

1. **NFR-01**: Login response must complete within 500ms (95th percentile)
2. **NFR-02**: Password hashing must use PBKDF2 with 100,000 iterations
3. **NFR-03**: Refresh token must be cryptographically random (64 bytes)
4. **NFR-04**: JWT access token must expire in 15 minutes
5. **NFR-05**: Refresh token must expire in 7 days
6. **NFR-06**: All errors must return generic messages (no credential enumeration)

## Acceptance Criteria

- [X] User can login with valid credentials and receives JWT + refresh token
- [X] Login fails with "Invalid credentials" for wrong username
- [X] Login fails with "Invalid credentials" for wrong password
- [X] Login fails with "Invalid tenant" for non-existent tenant code
- [X] Login fails with "User locked" for locked accounts
- [ ] Account is locked after 5 failed password attempts
- [X] `FailedAttempts` resets to 0 on successful login
- [X] `LastLogin` is updated on successful login
- [X] JWT contains all required claims (sub, tenant, company, role, perm, jti)
- [X] Refresh token hash is stored in database
- [X] Response includes userId, tenantId, companyId
- [X] Error responses do not reveal whether username exists

## Security Requirements

1. Passwords must never be logged
2. Failed login attempts must be logged with username and IP
3. Successful logins must be logged with user ID
4. Password verification must use `IPasswordHasher`, never direct comparison
5. Refresh token raw value is returned once; only hash is stored

## Tenant Isolation Requirements

1. Tenant must be resolved from `tenantCode` before any user lookup
2. User query must be scoped to the resolved tenant via EF Core query filter
3. Cross-tenant login attempts must be rejected

## API Contract

### POST /auth/login

**Request:**
```json
{
  "username": "string",
  "password": "string",
  "tenantCode": "string"
}
```

**Response (200):**
```json
{
  "accessToken": "string (JWT)",
  "refreshToken": "string",
  "expiresAt": "2026-05-29T23:30:00Z",
  "userId": "guid",
  "tenantId": "guid",
  "companyId": "guid"
}
```

**Error Responses:**
- `400`: "Invalid tenant"
- `400`: "Invalid credentials"
- `400`: "User locked"
- `500`: "Internal server error"

## Database Impact

- Reads from: `auth_users`, `auth_user_companies`, `auth_user_roles`, `auth_role_permissions`, `auth_tenants`
- Writes to: `auth_users` (FailedAttempts, LastLogin, IsLocked), `auth_refresh_tokens` (new record)

## Risks

1. **Password hashing performance**: PBKDF2 with 100K iterations adds ~200ms to login. Acceptable for authentication.
2. **Tenant resolution**: Extra DB call to resolve tenant by code. Could be cached if performance becomes an issue.

## Dependencies

- `IUserRepository` - User lookup and role/permission retrieval
- `IRefreshTokenRepository` - Refresh token storage
- `ITenantRepository` - Tenant resolution by code
- `IPasswordHasher` - Password verification
- `IJwtProvider` - JWT generation
- `IRefreshTokenService` - Refresh token generation
- `ICurrentUser` - Tenant context management
