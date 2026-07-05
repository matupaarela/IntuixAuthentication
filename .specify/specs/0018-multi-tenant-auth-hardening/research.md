# Research: Multi-Tenant Auth Hardening

## Technical Context

| Field | Decision |
| --- | --- |
| Runtime / Platform | .NET 8 ASP.NET Core Web API |
| Dependencies | MediatR, EF Core 8, SQL Server, JWT Bearer auth, Swashbuckle, FluentValidation, xUnit |
| Storage | SQL Server auth schema with `auth_*` tables |
| Testing | xUnit architecture tests plus unit/integration coverage |
| Target Platform | Server-side web service |
| Project Type | Multi-tenant authentication and authorization service |
| Performance Goals | p95 sign-in/refresh <= 2s; p95 session ops <= 1s |
| Constraints | Generic errors, manual unlock only, no MFA/OAuth SSO/API keys/admin CRUD |
| Scale / Scope | Intuix ecosystem trust boundary for auth/session flows |

## Design Decisions

### Durable lockout state

- Decision: persist `FailedAttempts`, `IsLocked`, and `LastLogin` on the user row and save them whenever sign-in succeeds or fails.
- Rationale: the current flow mutates lockout state in memory but does not persist the failure path, so lockout is not durable.
- Alternatives considered: cache-based lockout, separate lockout table.

### Refresh-token reuse handling

- Decision: treat refresh-token rows as session family records and revoke the full chain when a revoked token is reused.
- Rationale: the schema already carries `ReplacedByToken`, `RevokedAt`, and device metadata, which is enough to trace the family.
- Alternatives considered: add a separate session-family column, revoke all tokens for the user.

### Tenant and company enforcement

- Decision: trust tenant only from JWT/current-user state after login; company switching must validate same-tenant ownership, active company/organization state, and user membership.
- Rationale: this keeps tenant isolation fail-closed and matches the current architecture.
- Alternatives considered: pass tenant/company IDs in request bodies, perform ad hoc checks in controllers.

### Permission enforcement

- Decision: apply explicit endpoint policies from permission claims rather than role checks in controllers.
- Rationale: the policy pipeline already exists and keeps authorization consistent.
- Alternatives considered: role checks in controllers, per-handler claim parsing.

### Error handling

- Decision: centralize exception-to-ProblemDetails mapping and return generic security failures to clients.
- Rationale: current raw exceptions leak tenant/token state and can turn business failures into 500s.
- Alternatives considered: per-handler try/catch, controller-level error translation.

### Session tracking

- Decision: keep refresh tokens as the session record, add `last_used_at` for recency, and use the JWT `sid` claim as the current-session marker.
- Rationale: device/session views need a stable current-session signal and explicit last-use tracking.
- Alternatives considered: separate session table, infer current session from the latest active token.

### Manual-unlock audit ownership

- Decision: support/operations tooling owns manual unlock actions and emits the matching structured audit event; the auth service does not expose a public unlock endpoint.
- Rationale: keeps the operational path explicit without broadening the auth API surface.
- Alternatives considered: public manual-unlock endpoint, internal support command inside the auth service.

## Constitution Notes

- One controlled deviation is documented in the plan: `IgnoreQueryFilters()` for refresh-token lookup before tenant context exists.
- All other known gaps are planned as fixes rather than accepted exceptions.
