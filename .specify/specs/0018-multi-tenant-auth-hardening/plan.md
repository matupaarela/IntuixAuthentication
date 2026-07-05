# Implementation Plan: Multi-Tenant Auth Hardening

**Branch**: `0018-multi-tenant-auth-hardening` | **Date**: 2026-07-05 | **Spec**: `.specify/specs/0018-multi-tenant-auth-hardening/spec.md`

**Input**: Feature specification from `.specify/specs/0018-multi-tenant-auth-hardening/spec.md`

## Summary

Harden the auth surface by making lockout durable, revoking refresh families on reuse, enforcing tenant/company membership and permission policies, standardizing generic security failures, and preserving current session/device controls without changing the tenant model.

## Technical Context

**Language/Version**: C# / .NET 8

**Primary Dependencies**: ASP.NET Core Web API, MediatR, Entity Framework Core 8, SQL Server, JWT Bearer auth, Swashbuckle, xUnit, FluentValidation

**Storage**: SQL Server auth schema (`auth_*` tables) with EF Core configurations, forward-only migrations, and a checked-in SQL baseline

**Testing**: xUnit architecture tests plus unit/integration coverage for login, refresh reuse, company switching, device sessions, and security errors

**Target Platform**: Server-side web service (`Intuix.Authentication.Api`)

**Project Type**: Multi-tenant authentication and authorization service

**Performance Goals**: p95 sign-in/refresh <= 2s; p95 session operations <= 1s; 100% cross-tenant denial in regression

**Constraints**: Generic security errors only, manual unlock only, no MFA/OAuth SSO/API keys/admin CRUD, preserve existing users, keep `Api -> Application -> Domain`, allow only one controlled query-filter bypass for refresh-token lookup before tenant context exists

**Scale/Scope**: Intuix ecosystem trust boundary for tenant-scoped sign-in, token renewal, logout, company switching, and session/device controls

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Feature scope, plan, and source layout honor `Api -> Application -> Domain`.
- The feature lives under `.specify/specs/0018-multi-tenant-auth-hardening/` and follows the canonical naming scheme.
- Security, tenant isolation, and sensitive-data handling are explicitly addressed.
- Required tests, migrations, logging, and documentation impacts are identified.
- The only controlled deviation is a documented `IgnoreQueryFilters()` lookup for refresh-token resolution before tenant context exists; it is constrained and revalidated in the plan.

## Migration and Rollback Strategy

- `Intuix.Authentication.Infrastructure/Scripts/Intuix.Authentication.sql` remains the baseline for fresh installs.
- `Intuix.Authentication.Infrastructure/Migrations/` contains additive EF Core changes layered on top of that baseline.
- The feature migration is additive: backfill `LastUsedAt`, align session indexes, and avoid destructive schema changes.
- Rollout order is schema first, application second, then backfill and verification; success is not declared until active-session data is populated.
- Rollback is application-first: revert the API/Application binaries if needed and keep the additive schema in place so older binaries continue to run safely.
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

**Structure Decision**: Keep all feature work in `.specify/specs/0018-multi-tenant-auth-hardening/` and implement source changes in the existing repository layout defined by the constitution.

## Complexity Tracking

Only a single controlled exception is required.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| `IgnoreQueryFilters()` for refresh-token lookup before tenant context exists | Refresh requests need token lookup before ownership can be validated | Requiring tenant context first would block valid renewals and reuse detection |
