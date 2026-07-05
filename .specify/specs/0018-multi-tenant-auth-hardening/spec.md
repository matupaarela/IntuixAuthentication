# Feature Specification: Multi-Tenant Auth Hardening

**Feature Branch**: `[0018-multi-tenant-auth-hardening]`

**Created**: 2026-07-02

**Status**: Draft

**Input**: User description: "define lo conversado y lo hallado por mejorar en una sdd completa"

## Business Context

Intuix.Authentication is the trust boundary for the Intuix ecosystem. It authenticates users, controls company context, and protects tenant data. The current MVP already supports sign-in, token renewal, logout, company switching, and session listing, but the review uncovered gaps that must be closed before the platform can be treated as production-grade: failed login state is not reliably persisted, token reuse does not invalidate the full session family, company switching needs stricter membership validation, endpoint authorization is not consistently enforced by permission, and security failures are not handled in a uniform way.

This feature hardens the existing authentication surface without expanding into new product modules.

## Clarifications

### Session 2026-07-02

- Q: Should locked accounts unlock automatically or require support intervention? → A: Manual release by support/operations only.
- Q: Should switch-company return only an access token or the full auth envelope? → A: Full auth envelope.
- Q: Is `lastUsedAt` required, exposed in `/api/devices`, and updated on each successful refresh-token exchange? → A: Yes.
- Q: Should existing locked accounts be auto-unlocked during rollout? → A: No. Existing lock state is preserved and support/operations must release it manually.

### Session 2026-07-05

- Q: Where should manual-unlock audit events be emitted? → A: By the support/operations process outside the auth service, with a documented schema and no new public endpoint.

## Problem Statement

The platform works on the happy path, but several failure paths still allow inconsistent security behavior or unclear user outcomes. In a shared multi-tenant environment, that creates risk of account lockouts not taking effect, stale sessions surviving reuse, users selecting companies they should not access, and protected endpoints relying on incomplete enforcement.

The service needs a complete, testable contract for safe authentication and tenant isolation before more capabilities are added.

## Goals

1. Make login state changes durable and predictable after failures.
2. Detect refresh-token reuse and revoke the affected session family.
3. Enforce tenant and company boundaries on every protected action.
4. Apply permission-based access control consistently at the endpoint level.
5. Keep session visibility and revocation controls usable for end users.
6. Return generic, non-enumerating security failures.
7. Preserve compatibility with current signed-in users during rollout.

## Non Goals

- Adding multi-factor authentication.
- Adding OAuth or SSO.
- Adding API key authentication.
- Building tenant, organization, company, user, role, or permission administration CRUD.
- Introducing a new audit-log product module.
- Building a public manual-unlock endpoint.
- Redesigning the client applications or user interface.
- Changing the tenant hierarchy or business model.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Secure Sign-In and Lockout (Priority: P1)

A tenant user signs in with a username, password, and tenant code. Successful sign-in returns a new session, while repeated failures lock the account until it is released manually by support/operations.

**Why this priority**: Sign-in is the entry point for all other capabilities, and lockout behavior is a core security control.

**Independent Test**: Can be tested by attempting valid and invalid sign-ins for the same tenant and verifying that lockout occurs after the configured number of failures.

**Acceptance Scenarios**:

1. **Given** an active user in the correct tenant, **When** the correct credentials are submitted, **Then** the user receives a valid session and the default company context.
2. **Given** repeated invalid passwords for the same account, **When** the failure threshold is reached, **Then** the account is locked and later attempts are rejected.
3. **Given** a username that exists in another tenant, **When** the user signs in against the wrong tenant code, **Then** the sign-in is denied without revealing whether the account exists elsewhere.

---

### User Story 2 - Session Renewal and Reuse Detection (Priority: P1)

A signed-in user renews an active session with a refresh token. Each renewal must issue a new session token pair, and any reuse of a revoked token must invalidate the related session family.

**Why this priority**: Session renewal is the highest-risk security path after sign-in, and reuse detection is required to contain token theft.

**Independent Test**: Can be tested by using a refresh token once successfully, then attempting to reuse the old token and verifying that the full family is revoked.

**Acceptance Scenarios**:

1. **Given** a valid active refresh token, **When** it is used to renew the session, **Then** a new access token and a new refresh token are issued.
2. **Given** a refresh token that has already been replaced or revoked, **When** it is used again, **Then** the entire related session family is revoked.
3. **Given** an expired refresh token, **When** the user attempts renewal, **Then** the request is rejected with a generic security message.

---

### User Story 3 - Tenant and Permission Enforcement (Priority: P1)

Protected endpoints are available only to users who belong to the active tenant and have the required permission for the action.

**Why this priority**: Tenant isolation and permission enforcement are the core trust guarantees of the platform.

**Independent Test**: Can be tested by calling protected endpoints with missing permissions or with tenant data that does not belong to the current user.

**Acceptance Scenarios**:

1. **Given** a user missing the required permission, **When** they call a protected endpoint, **Then** access is denied.
2. **Given** a request that would cross tenant boundaries, **When** it is processed, **Then** it is rejected.
3. **Given** a request missing trusted tenant context, **When** it reaches a protected boundary, **Then** it fails closed.

---

### User Story 4 - Company Switching and Session Controls (Priority: P2)

A user who belongs to more than one company can switch context, inspect active device sessions, and revoke sessions that belong to their own account.

**Why this priority**: Multi-company users need control over their working context and active sessions, but these actions must remain tenant-safe.

**Independent Test**: Can be tested by assigning a user to multiple companies, switching company context, listing active sessions, and revoking one or all sessions.

**Acceptance Scenarios**:

1. **Given** a user assigned to multiple companies in one tenant, **When** they switch companies, **Then** the standard auth response envelope is returned with the selected company context, a new access token, and an empty refresh token field.
2. **Given** a company not assigned to the user, is inactive, belongs to an inactive organization, or is outside the tenant, **When** it is selected, **Then** the request is rejected.
3. **Given** multiple active device sessions, **When** the user lists sessions, **Then** they see each session with device metadata and the current session is marked.
4. **Given** a specific session, **When** the user revokes it, **Then** only that session is removed.
5. **Given** the current session, **When** the user revokes all others, **Then** the current session remains active.

### Edge Cases

- The tenant code resolves to an inactive tenant.
- The user is inactive or already locked.
- The user has no default company assignment.
- The refresh token is missing, malformed, expired, revoked, or reused after logout.
- The selected company is active but belongs to another tenant.
- The selected company is inactive or its parent organization is inactive.
- Session listing is requested after all sessions have already been revoked.
- Session history is requested for a legacy row where `lastUsedAt` has not yet been backfilled.
- A protected operation is attempted without the required permission context.
- A persistence failure occurs after validation but before commit; the operation fails closed and returns a generic error.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system must accept username, password, and tenant code as sign-in inputs.
- **FR-002**: The system must resolve the tenant before looking up the user.
- **FR-003**: The system must reject unknown or inactive tenants.
- **FR-004**: The system must reject inactive or locked users.
- **FR-005**: The system must track consecutive failed sign-in attempts and lock the account after 5 consecutive failed attempts; locked accounts must remain locked until support/operations manually release them.
- **FR-006**: The system must reset the failure counter after a successful sign-in.
- **FR-007**: The system must update the user's last successful login time after a successful sign-in.
- **FR-008**: The system must return a signed access token and a refresh token on successful sign-in.
- **FR-009**: The access token must carry the user's identity, tenant, company, role, permission, and session context via the JWT `sid` claim.
- **FR-010**: The system must assign the user's default company at sign-in when one exists.
- **FR-011**: The system must reject sign-in when no company assignment exists.
- **FR-012**: The system must renew the session only with a valid, unrevoked refresh token.
- **FR-013**: The system must rotate refresh tokens on each renewal.
- **FR-014**: The system must revoke the entire related session family when a revoked refresh token is presented again.
- **FR-015**: The system must support logout for a specific session and logout of all sessions for the current user.
- **FR-016**: The system must allow a user to list active device sessions with device metadata and current-session status.
- **FR-017**: The system must allow a user to revoke a specific active session.
- **FR-018**: The system must allow a user to revoke all sessions except the current one.
- **FR-019**: The system must allow company switching only to active companies assigned to the user and within the same tenant; the owning organization must also be active.
- **FR-020**: The system must deny protected operations when the required permission is missing.
- **FR-021**: The system must enforce tenant boundaries for all tenant-scoped data and session operations.
- **FR-022**: The system must return generic error messages for invalid tenants, invalid credentials, locked users, missing or malformed refresh tokens, expired sessions, unauthorized company selection, and reused tokens.
- **FR-023**: The system must capture session metadata needed for user visibility and revocation decisions, including IP and user agent where available.
- **FR-024**: The system must preserve token and session history needed to trace rotation and revocation chains.
- **FR-025**: The system must record security-relevant outcomes with tenant and session context, including manual-unlock audit events emitted by support/operations tooling, without exposing credentials, secrets, or token values.
- **FR-026**: The system must update the session `lastUsedAt` value on each successful refresh-token exchange, expose it in active-session listings, and backfill legacy active rows from `CreatedAt` during migration.
- **FR-027**: The system must return the standard auth response envelope on successful company switch, including the selected company context, a new access token, and an empty refresh-token field.
- **FR-028**: The system must validate authentication and session inputs before applying persistence or state changes.
- **FR-029**: The system must produce structured logs for sign-in, refresh, company switch, revocation, and manual-unlock flows with correlation to tenant, user, session, and request context.
- **FR-030**: The system must commit login, refresh, logout, logout-all, company-switch, and session-revocation state changes atomically; if persistence fails, the request must fail closed and no success response may be returned.

### Non-Functional Requirements

- **NFR-001**: 95% of sign-in and session-renewal actions must complete within 2 seconds under expected load.
- **NFR-002**: 95% of session-listing, session-revocation, and company-switch actions must complete within 1 second under expected load.
- **NFR-003**: 100% of cross-tenant access attempts must fail in acceptance and regression testing.
- **NFR-004**: 100% of security failures must use generic messages and must not reveal whether a tenant, user, or token exists.
- **NFR-005**: 100% of protected endpoints must deny access when permission or tenant context is missing or invalid.
- **NFR-006**: The rollout must not force existing users to re-register or change credentials.
- **NFR-007**: Transient persistence, logging, or token-store failures must not create partial auth state; retries must remain safe and the service must fail closed with generic errors.
- **NFR-008**: Auth failure and recovery paths must continue returning generic responses under transient downstream outages without hanging, leaking secrets, or exposing partial session state.

### Security Requirements

1. Passwords must never be stored or compared in plain text.
2. Refresh tokens must never be stored or exposed in recoverable form.
3. Reused or revoked refresh tokens must invalidate the related session family.
4. Access tokens must be short-lived and renewal must only be possible through a valid refresh token.
5. Company selection must be validated against both user membership and tenant ownership.
6. Sensitive secrets must never be logged or returned to clients.
7. Permission checks must be enforced consistently at the endpoint boundary.
8. Security-related events must be traceable by user and session without exposing credentials.
9. Sign-in failures, lockouts, refresh reuse, company switches, session revocations, and manual unlocks must emit structured operational audit events with tenant, user, session, action, outcome, and correlation context, without credentials, secrets, or token values.

### Tenant Isolation Requirements

Tenant isolation is mandatory. Every tenant-scoped concept must remain visible only inside the current tenant.

| Concept | Isolation rule |
| --- | --- |
| User | Visible only inside the owning tenant |
| Role | Visible only inside the owning tenant |
| Organization | Visible only inside the owning tenant |
| Company | Visible only through its parent organization's tenant |
| Membership links | Visible only when the associated user or role belongs to the current tenant |
| Session records | Visible only when the associated user belongs to the current tenant |

- Tenant context must come from trusted authentication state, not from request body values.
- Company switching must never cross tenant boundaries.
- Session listing and revocation must never affect accounts in another tenant.
- Any request without a trusted tenant context must fail closed.

### API Contracts

#### POST /auth/login

Request fields: username, password, tenantCode.

Response fields on success: accessToken, refreshToken, expiresAt, userId, tenantId, companyId.

Failure response: generic authentication error.

#### POST /auth/refresh

Request fields: refreshToken.

Response fields on success: new accessToken, new refreshToken, expiresAt, userId, tenantId, companyId.

Failure response: generic session error for expired, revoked, or reused tokens.

#### POST /auth/logout

Request fields: refreshToken.

Response on success: confirmation that the related session was revoked.

#### POST /auth/logout-all

Response on success: confirmation that all sessions for the current user were revoked.

#### POST /auth/switch-company

Request fields: companyId.

Response on success: the standard auth response envelope with the selected company context, a new access token, and an empty refresh-token field.

Failure response: generic unauthorized-company error.

#### GET /api/devices

Response on success: the list of active sessions for the current user, including device metadata and current-session status.

#### DELETE /api/devices/{tokenId}

Response on success: confirmation that the selected session was revoked.

#### POST /api/devices/revoke-all

Response on success: confirmation that all other sessions were revoked.

### Database Impact

- Existing user records must persist failed-attempt state, lock state, and last successful login time reliably.
- Existing refresh-session records must persist rotation history, revocation state, device metadata, and `lastUsedAt` reliably.
- Existing active refresh-session rows must be backfilled so `lastUsedAt` is available before rollout is considered complete.
- Existing company membership data must continue to support default-company and allowed-company checks.
- Lookup performance must be supported by stable indexes for tenant code, username, membership checks, and active sessions.
- No new top-level identity tables are required for this phase.

### Key Entities

- **Tenant**: The top-level security boundary for the entire platform.
- **Organization**: A business grouping that belongs to one tenant.
- **Company**: The operational context a user can switch into.
- **User**: A person authenticated by the platform and tied to one tenant.
- **Role**: A tenant-scoped access grouping used to grant permissions.
- **Permission**: A capability that can be granted to a role.
- **User-Company membership**: The allowed-company relationship for a user, including the default company.
- **Refresh session**: The active session family that can be renewed, rotated, and revoked; the JWT `sid` claim identifies the current session.
- **Device session**: The user-facing refresh-session view that includes device metadata, recency, and current-session status, keyed by the `sid`-backed current session marker.

### Risks

1. Stricter checks may expose existing data-quality issues in memberships or default-company assignments.
2. Lockout behavior may increase support contacts if there is no clear manual unlock path.
3. Reuse detection may force users to re-authenticate when older clients retry with stale tokens.
4. Endpoint permission enforcement may temporarily block workflows that were previously allowed implicitly.
5. Session hardening may require client updates to handle generic failures and forced re-authentication.

### Dependencies

- Existing tenant, company, role, permission, and membership data.
- Signed access-token configuration and session-expiry settings.
- An operational path to manually unlock users and maintain company memberships.
- Support/operations tooling owns manual unlock actions and emits the corresponding audit events.
- An operational logging sink that can retain structured security and manual-unlock events.
- Client applications that can handle refresh rotation and generic security failures.
- Regression coverage for tenant boundaries and session management.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of lockout acceptance tests pass, including the 5-attempt failure threshold.
- **SC-002**: 100% of refresh-token reuse acceptance tests revoke the full related session family.
- **SC-003**: 0 successful cross-tenant access attempts occur in regression and UAT for login, refresh, switch-company, and device/session flows.
- **SC-004**: 95% of sign-in, refresh, company-switch, and session-control actions complete within 2 seconds under expected load.
- **SC-005**: 90% of pilot users in the designated rollout cohort can sign in, switch company, and revoke a session without assistance.
- **SC-006**: Support tickets related to wrong company context or stale session behavior drop by at least 50% compared with the 30-day pre-launch baseline after rollout.

## Assumptions

- Existing users will continue using the current sign-in path.
- Existing locked users remain locked during rollout until manually released; the deployment does not auto-unlock them.
- Refresh-token revocation ends renewal ability, while the access token expires naturally.
- Client applications understand refresh-token rotation and generic security failures, and stale refresh tokens may require re-authentication.
- Users may belong to multiple companies inside one tenant, and one company is the default.
- Multi-factor authentication, OAuth/SSO, and API keys are handled as separate future features; the separate audit-log product module remains out of scope, but this feature still emits its required security and manual-unlock operational logs.
- Seed data and existing examples will be updated to stay coherent with the new rules.
