# 0016-device-management - Specification

## Business Context

Users authenticate from multiple devices. Device management tracks which devices have active sessions, when they last connected, and allows users to revoke sessions on specific devices. This enhances security by enabling users to identify and terminate unauthorized sessions.

## Problem Statement

Currently, refresh tokens capture IP and User-Agent but there is no way for users to view their active sessions or revoke specific device sessions. Device management provides visibility and control over active sessions.

## Goals

1. List active sessions (devices) for a user
2. View session details (device info, last activity)
3. Revoke a specific device session
4. Revoke all sessions except current

## Non-Goals

- Device fingerprinting
- Device trust management
- Push notifications to devices
- Device-specific policies

## Functional Requirements

1. **FR-01**: List all active refresh tokens for the current user
2. **FR-02**: Display device info (IP, User-Agent, created date, last used)
3. **FR-03**: Revoke a specific device session by token ID
4. **FR-04**: Revoke all sessions except the current one
5. **FR-05**: Current session is identified by the active refresh token
6. **FR-06**: Require authentication for all operations

## Non-Functional Requirements

1. **NFR-01**: Session list must load within 200ms
2. **NFR-02**: Revocation must be atomic

## Acceptance Criteria

- [X] GET /api/devices returns list of active sessions
- [X] Each session shows IP, User-Agent, created date
- [X] DELETE /api/devices/{tokenId} revokes specific session
- [X] POST /api/devices/revoke-all revokes all except current
- [X] Current session is not included in revoke-all
- [X] Revoked sessions disappear from list

## API Contract

### GET /api/devices

**Headers:** `Authorization: Bearer {accessToken}`

**Response (200):**
```json
{
  "sessions": [
    {
      "tokenId": "guid",
      "ipAddress": "192.168.1.1",
      "userAgent": "Mozilla/5.0...",
      "createdAt": "2026-05-29T10:00:00Z",
      "isCurrent": true
    }
  ]
}
```

### DELETE /api/devices/{tokenId}

**Response (200):**
```json
{
  "message": "Session revoked"
}
```

### POST /api/devices/revoke-all

**Response (200):**
```json
{
  "message": "All other sessions revoked"
}
```

## Database Impact

- Reads from: `auth_refresh_tokens`
- Writes to: `auth_refresh_tokens` (RevokedAt)

## Dependencies

- `IRefreshTokenRepository`
- `ICurrentUser`

