# Implementation Plan: Multi-Tenant Auth Hardening

**Branch**: `0018-multi-tenant-auth-hardening` | **Date**: 2026-07-02 | **Spec**: `.specify/specs/0018-multi-tenant-auth-hardening/spec.md`

**Input**: Feature specification from `.specify/specs/0018-multi-tenant-auth-hardening/spec.md`

## Summary

Harden the auth surface by making lockout durable, revoking refresh families on reuse, enforcing tenant/company membership and permission policies, standardizing security failures, and preserving existing session/device controls.

## Technical Context

**Language/Version**: C# / .NET 8

**Primary Dependencies**: ASP.NET Core Web API, MediatR, Entity Framework Core 8, SQL Server, JWT Bearer auth, Swashbuckle, xUnit

**Storage**: SQL Server auth schema (`auth_*` tables) with EF Core configurations and a checked-in SQL baseline

**Testing**: xUnit architecture tests plus unit/integration tests for login, refresh reuse, company switching, device sessions, and security errors

**Validation**: FluentValidation validators for auth and session inputs

**Observability**: Structured security logging with tenant and session correlation, plus manual-unlock audit events

**Target Platform**: Server-side web service (`Intuix.Authentication.Api`)

**Project Type**: Multi-tenant authentication and authorization service

**Performance Goals**: p95 sign-in/refresh <= 2s; p95 session operations <= 1s; 100% cross-tenant denial in regression

**Constraints**: Generic security errors only, manual unlock only, no MFA/OAuth SSO/API keys/admin CRUD, preserve existing users, keep `Api -> Application -> Domain`

**Scale/Scope**: Intuix ecosystem trust boundary for tenant-scoped sign-in, token renewal, logout, company switching, and session/device controls

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Feature scope, plan, and source layout honor `Api -> Application -> Domain`.
- The feature lives under `.specify/specs/0018-multi-tenant-auth-hardening/` and follows the canonical naming scheme.
- Security, tenant isolation, and sensitive-data handling are explicitly addressed.
- Required tests, validators, migrations, logging, and documentation impacts are identified.
- A controlled `IgnoreQueryFilters()` refresh-token lookup is documented as a deviation and must be revalidated against the owning user before issuance.

## Migration and Rollout Strategy

- `Intuix.Authentication.Infrastructure/Scripts/Intuix.Authentication.sql` is the checked-in baseline schema for fresh installs.
- `Intuix.Authentication.Infrastructure/Migrations/` holds forward-only EF Core changes layered on top of that baseline.
- The feature migration is additive: backfill `LastUsedAt`, align session indexes, and avoid destructive schema changes.
- Rollout order is schema first, application second, then backfill/verification; success is not declared until the active-session data is populated.
- Rollback is application-first: revert the API/application binaries if needed and keep the additive schema in place so older binaries continue to run safely.
- The only tolerated query-filter bypass is refresh-token lookup before tenant context exists, and ownership must be revalidated before issuance or revocation.

## Project Structure

### Documentation (this feature)

```text
.specify/specs/0018-multi-tenant-auth-hardening/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
Intuix.Authentication.Api/
├── Authorization/
├── Controllers/
├── Middleware/
├── Program.cs
└── Swagger/

Intuix.Authentication.Application/
├── Auth/
│   ├── Commands/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Validators/
├── Devices/
│   ├── Commands/
│   ├── DTOs/
│   ├── Queries/
│   └── Validators/
└── Common/

Intuix.Authentication.Domain/
├── Entities/
└── Interfaces/

Intuix.Authentication.Infrastructure/
├── Persistence/
├── Scripts/
└── Security/

tests/
└── Intuix.Authentication.ArchitectureTests/
```

**Structure Decision**: Use the active `.specify/specs/0018-multi-tenant-auth-hardening/` directory as the feature workspace and keep source code in the repository root solution layout defined by the constitution.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Refresh-token lookup bypasses query filters before tenant context exists | Refresh requests need token lookup before ownership can be validated | Requiring tenant context first would block valid renewals and reuse detection |
