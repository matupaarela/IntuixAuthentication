# logout-and-revoke-chain - Specification

## Business Context

Users need to securely end their sessions. Logout must revoke the current refresh token and, optionally, all refresh tokens across all devices. This prevents stolen tokens from being used after the user logs out.

## Problem Statement

The current `Revoke` endpoint in `AuthController` is a stub that returns a placeholder message. It must be implemented to revoke refresh tokens. Additionally, a "logout all" endpoint is needed to revoke all tokens for a user across all devices.

## Goals

1. Implement single-device logout (revoke current refresh token)
2. Implement all-device logout (revoke all user refresh tokens)
3. Revoke token chains on logout
4. Return appropriate HTTP status codes

## Non-Goals

- Access token blacklisting (use short expiry instead)
- Device management UI (separate feature)
- Session management dashboard

## Functional Requirements

1. **FR-01**: The system must accept a refresh token as input for single-device logout
2. **FR-02**: The system must hash the token and look it up
3. **FR-03**: The system must mark the token as revoked (`RevokedAt = now`)
4. **FR-04**: The system must revoke the entire token chain from the revoked token onward
5. **FR-05**: The system must return 200 on successful revocation
6. **FR-06**: For logout-all, the system must revoke ALL active refresh tokens for the user
7. **FR-07**: The system must require authentication for logout-all
8. **FR-08**: Logout must be idempotent (revoking an already-revoked token succeeds)

## Non-Functional Requirements

1. **NFR-01**: Logout must complete within 100ms
2. **NFR-02**: Logout-all must revoke all tokens atomically

## Acceptance Criteria

- [X] POST /auth/logout with valid refresh token → 200, token revoked
- [X] POST /auth/logout with already-revoked token → 200 (idempotent)
- [X] POST /auth/logout with expired token → 200 (idempotent)
- [X] POST /auth/logout-all with valid auth → 200, all user tokens revoked
- [X] POST /auth/logout-all without auth → 401
- [X] Token chain is revoked on logout
- [X] Logout-all revokes all active tokens for user

## Security Requirements

1. Logout must be idempotent to prevent token enumeration
2. Logout-all must require authentication
3. Revocation events must be logged

## Tenant Isolation Requirements

1. Token revocation is scoped to the user (implicitly tenant-scoped)

## API Contract

### POST /auth/logout

**Request:**
```json
{
  "refreshToken": "string"
}
```

**Response (200):**
```json
{
  "message": "Logged out successfully"
}
```

### POST /auth/logout-all

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response (200):**
```json
{
  "message": "All sessions terminated"
}
```

## Database Impact

- Writes to: `auth_refresh_tokens` (RevokedAt)

## Risks

1. **Race condition**: Token could be used between validation and revocation. Mitigated by atomic operations.

## Dependencies

- `IRefreshTokenRepository` - Token lookup and revocation
- `ICurrentUser` - User identification for logout-all
