# refresh-token-rotation - Specification

## Business Context

Refresh tokens enable users to obtain new access tokens without re-authenticating. The system must implement refresh token rotation where each use of a refresh token revokes it and issues a new one, creating a chain. This prevents token theft from being reused indefinitely.

## Problem Statement

The current implementation performs basic rotation but lacks chain revocation on reuse detection, device tracking metadata capture, and proper token family management. A reused (revoked) token must trigger revocation of the entire token family to mitigate stolen token attacks.

## Goals

1. Rotate refresh tokens on every use (revoke old, issue new)
2. Maintain token replacement chain (`ReplacedByToken` field)
3. Detect token reuse and revoke entire token family
4. Capture device metadata (IP address, User-Agent)
5. Enforce 7-day expiry on refresh tokens

## Non-Goals

- Device management UI (separate feature: device-management)
- Token blacklist for access tokens
- Multi-device session management UI

## Functional Requirements

1. **FR-01**: The system must accept a refresh token as input
2. **FR-02**: The system must hash the token with SHA256 and look it up in the database
3. **FR-03**: The system must reject if token is not found
4. **FR-04**: The system must detect if token is revoked (`RevokedAt` is not null)
5. **FR-05**: If token is revoked, the system must detect a reuse attack and revoke the entire token family
6. **FR-06**: The system must reject if token is expired (`ExpiresAt` < now)
7. **FR-07**: The system must verify the user is active
8. **FR-08**: The system must revoke the current token (`RevokedAt = now`)
9. **FR-09**: The system must generate a new refresh token
10. **FR-10**: The system must link the old token to the new token (`ReplacedByToken`)
11. **FR-11**: The system must store the new refresh token hash
12. **FR-12**: The system must issue a new JWT access token with current permissions
13. **FR-13**: The system must capture IP address and User-Agent from the request

## Non-Functional Requirements

1. **NFR-01**: Refresh operation must complete within 300ms
2. **NFR-02**: Token hash must be SHA256
3. **NFR-03**: Refresh token expiry must be 7 days
4. **NFR-04**: Chain revocation must be atomic (single transaction)

## Acceptance Criteria

- [X] Valid refresh token returns new access token + new refresh token
- [X] Old refresh token is marked as revoked
- [X] Old token's `ReplacedByToken` points to new token ID
- [X] Expired refresh token is rejected
- [ ] Revoked refresh token triggers family revocation
- [ ] Reuse detection revokes all tokens in the chain
- [ ] User's refresh tokens are all revoked on reuse detection
- [ ] IP address and User-Agent are captured
- [X] New JWT contains current user permissions

## Security Requirements

1. Refresh tokens must be stored hashed (SHA256)
2. Raw refresh token must be returned only once
3. Reuse of revoked token must trigger chain revocation
4. Token family must be traceable via `ReplacedByToken`

## Tenant Isolation Requirements

1. Refresh token lookup is scoped to user (implicitly tenant-scoped via User entity)
2. No cross-user token access

## API Contract

### POST /auth/refresh

**Request:**
```json
{
  "refreshToken": "string"
}
```

**Response (200):**
```json
{
  "accessToken": "string (JWT)",
  "refreshToken": "string (new)",
  "expiresAt": "2026-05-29T23:30:00Z",
  "userId": "guid",
  "tenantId": "guid",
  "companyId": "guid"
}
```

**Error Responses:**
- `400`: "Invalid refresh token"
- `400`: "Token expired"
- `400`: "Token already revoked (possible reuse attack)"
- `400`: "Invalid user"

## Database Impact

- Reads from: `auth_refresh_tokens` (with User join)
- Writes to: `auth_refresh_tokens` (RevokedAt, ReplacedByToken on old; new record)

## Risks

1. **Chain revocation performance**: Long token chains may require recursive queries. Mitigated by 7-day expiry limiting chain length.
2. **Race conditions**: Two concurrent refresh requests could both succeed. Mitigated by atomic revocation.

## Dependencies

- `IRefreshTokenRepository` - Token lookup and storage
- `IUserRepository` - User verification and company/role/permission retrieval
- `IJwtProvider` - JWT generation
- `IRefreshTokenService` - Token generation
