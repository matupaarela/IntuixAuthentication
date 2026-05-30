# switch-company - Specification

## Business Context

Users may belong to multiple companies within a tenant. They need to switch the active company context without re-authenticating. Switching company issues a new JWT with the new company claim while preserving the same refresh token.

## Problem Statement

The current `SwitchCompanyCommandHandler` works but lacks validation that the company belongs to the user's tenant, does not validate company existence, and does not return a refresh token in the response.

## Goals

1. Allow authenticated users to switch active company
2. Validate company belongs to user
3. Validate company exists and is active
4. Issue new JWT with updated company claim
5. Return updated tokens

## Non-Goals

- Company management (separate feature)
- Default company assignment
- Company creation

## Functional Requirements

1. **FR-01**: The system must require authentication
2. **FR-02**: The system must accept a `companyId` as input
3. **FR-03**: The system must verify the company exists and is active
4. **FR-04**: The system must verify the user is assigned to the company
5. **FR-05**: The system must verify the company belongs to the user's tenant
6. **FR-06**: The system must generate a new JWT with the new company claim
7. **FR-07**: The system must return `AuthResponse` with updated tokens
8. **FR-08**: The refresh token must not be rotated on company switch

## Non-Functional Requirements

1. **NFR-01**: Switch must complete within 200ms
2. **NFR-02**: New JWT must contain updated company claim

## Acceptance Criteria

- [ ] Authenticated user with valid company → 200 with new JWT
- [ ] Company not assigned to user → 400 "Unauthorized company"
- [ ] Non-existent company → 400 "Company not found"
- [ ] Inactive company → 400 "Company is inactive"
- [ ] Unauthenticated → 401
- [ ] New JWT contains the switched company ID
- [ ] Refresh token is not changed

## Security Requirements

1. Users can only switch to companies they are assigned to
2. Company must belong to user's tenant
3. Authentication is required

## Tenant Isolation Requirements

1. Company validation must ensure company belongs to user's tenant
2. Cross-tenant company switching is forbidden

## API Contract

### POST /auth/switch-company

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "companyId": "guid"
}
```

**Response (200):**
```json
{
  "accessToken": "string (new JWT)",
  "refreshToken": "",
  "expiresAt": "2026-05-29T23:30:00Z",
  "userId": "guid",
  "tenantId": "guid",
  "companyId": "guid (switched)"
}
```

**Error Responses:**
- `400`: "Company not found"
- `400`: "Company is inactive"
- `400`: "Unauthorized company"
- `401`: "Unauthorized"

## Database Impact

- Reads from: `auth_companies`, `auth_user_companies`
- No writes

## Risks

1. **Stale JWT**: Old JWT remains valid until expiry. Mitigated by 15-minute expiry.

## Dependencies

- `IUserRepository` - User company retrieval
- `ICompanyRepository` - Company validation
- `IJwtProvider` - JWT generation
- `ICurrentUser` - User context
