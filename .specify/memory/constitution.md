<!--
Sync Impact Report
Version: 1.1.0 -> 1.2.0
Modified principles:
- I. Clean Architecture Boundaries (expanded with composition-root and controller/handler rules)
- II. Feature-Scoped CQRS (expanded with file-layout, naming, DTO, validator, and repository boundaries)
- III. Security, Policy Authorization, and Tenant Isolation (expanded with password, token, lockout, and tenant rules)
- IV. Explicit Persistence and EF Core Discipline (expanded with repository, EF Core, migration, and query-performance rules)
- V. Testability, Observability, and Safe Change Management (expanded with testing, logging, definition of done, and guardrails)
Added sections:
- None
Removed sections:
- Vision
- Architectural Principles
- Folder Organization
- Naming Conventions
- Dependency Rules
- CQRS Rules
- Repository Rules
- DTO Rules
- Validation Rules
- Security Rules
- JWT Rules
- Refresh Token Rules
- Multi-Tenancy Rules
- EF Core Rules
- Migration Rules
- Testing Rules
- Logging Rules
- Definition of Done
- Anti-Patterns
- Forbidden Practices
- Amendment Process
Templates requiring updates:
- .specify/templates/plan-template.md ✅ reviewed; already aligned
- .specify/templates/spec-template.md ✅ reviewed; already aligned
- .specify/templates/tasks-template.md ✅ reviewed; already aligned
- .specify/specs/README.md ✅ updated rename catalog
Follow-up TODOs: none
-->

# Intuix.Authentication Constitution

## Core Principles

### I. Clean Architecture Boundaries

- Dependency flow MUST remain `Api -> Application -> Domain`.
- `Infrastructure` MAY only implement contracts defined by `Application` or `Domain`.
- `Domain` MUST never reference `Api`, `Application`, or `Infrastructure`.
- `Application` MUST never depend on `Infrastructure` or `Api`.
- `Infrastructure` MUST never be referenced by `Domain`.
- Controllers and handlers MUST stay free of reverse-layer dependencies.
- API controllers MUST remain thin and delegate business logic to application handlers.
- Dependency injection registration MUST live in the API composition root.

Rationale: Keeps security-critical code testable and prevents coupling across layers.

### II. Feature-Scoped CQRS

- Every feature MUST live in a feature folder and expose separate command/query paths.
- Commands and queries MUST each have exactly one handler.
- Commands and queries MUST implement MediatR `IRequest<TResponse>` and the matching handler contract.
- Feature code MUST keep DTOs, validators, and interfaces beside the feature.
- Feature folders MUST use `Commands/`, `Queries/`, `DTOs/`, `Validators/`, and `Interfaces/`.
- One class per file is required, and file names MUST match class names.
- Command and handler files MUST follow `[Command].cs` and `[Command]Handler.cs` naming.
- DTOs MUST be records or classes with public setters, contain no business logic, and live in the feature `DTOs/` folder.
- Queries MAY return DTOs directly.
- Commands MUST not return collections for write operations.
- Business rules SHOULD live in handlers or domain services; entities remain data-centric unless a richer model is explicitly justified.
- Generic repositories, global command/query folders, and shared DTO dumps are forbidden.
- Validators MUST live in feature `Validators/`; one validator per command/query is the rule.
- Complex validation SHOULD use FluentValidation.
- Validation MUST complete before persistence or outbound side effects.

Rationale: Makes changes predictable, isolated, and independently testable.

### III. Security, Policy Authorization, and Tenant Isolation

- Authentication MUST use hashed passwords, hashed refresh tokens, and config-driven JWT secrets.
- Passwords MUST be hashed with PBKDF2 + SHA256 and 100,000 iterations.
- Plain text passwords and direct password comparison are forbidden.
- `IPasswordHasher` MUST be used for verification.
- Authentication MUST enforce account lockout after 5 consecutive failed attempts.
- Authorization MUST use policies; role checks inside controllers are forbidden.
- Sensitive values such as passwords, tokens, secrets, and connection strings MUST never be logged or returned to clients.
- Every tenant-scoped entity MUST enforce tenant query filters.
- API requests MUST never accept tenant IDs from request bodies for trust decisions.
- Tenant resolution MUST come from JWT claims and `ICurrentUser`; `ICurrentUser.SetTenant()` is reserved for authentication flows.
- Access tokens MUST remain short-lived (15 minutes), signed with HMAC-SHA256, and configured externally.
- JWT claims MUST use `snake_case`; required claims include `sub`, `tenant`, `company`, `role`, `perm`, and `jti`.
- Refresh tokens MUST be cryptographically random, hashed before storage, rotated on use, and revoked on reuse.
- Reuse of a revoked refresh token MUST revoke the whole token family.
- Refresh tokens MUST expire after 7 days and capture device metadata such as IP address and User-Agent.
- Security failures MUST return generic messages; stack traces and detailed failure reasons MUST not reach clients.

Rationale: The platform exists to protect identities and tenant boundaries; failure here is a product defect.

### IV. Explicit Persistence and EF Core Discipline

- Entity mappings MUST use `IEntityTypeConfiguration<T>` and `ApplyConfigurationsFromAssembly`.
- Repository interfaces MUST live in Application feature folders.
- Repository implementations MUST live in `Infrastructure/Persistence/Repositories`.
- Repositories MUST be async, aggregate-specific, and return domain entities rather than DTOs.
- Repositories MUST accept `CancellationToken`.
- `IQueryable` MUST NOT leak from repository interfaces.
- Implementations MUST use `AuthDbContext` via constructor injection.
- Tenant-scoped entities MUST apply query filters using `ICurrentUser.TenantId`.
- Read-only queries SHOULD use `AsNoTracking`, projection, and eager loading only when required.
- `SaveChangesAsync` MUST be used for persistence.
- Migrations MUST be generated through the CLI, reviewed before merge, and seed data MUST be handled separately.
- Database tables MUST use `snake_case` with the `auth_` prefix; columns and JWT claims MUST use `snake_case`.
- Generic CRUD repositories, DataAnnotation persistence mapping, domain events, event sourcing, outbox pattern, and microservices are not part of this codebase unless explicitly re-approved.

Rationale: Persistence rules keep data access predictable, performant, and reviewable.

### V. Testability, Observability, and Safe Change Management

- Features that touch auth, tenancy, security, or persistence MUST include unit and integration coverage.
- Architecture tests MUST verify dependency direction, folder rules, naming conventions, and repository constraints.
- Structured logging with correlation and tenant context is mandatory; debug logging is environment-only.
- Logging MUST cover security-relevant and operational events without exposing secrets or high-risk PII.
- Changes MUST be implemented in the smallest safe slice and documented with versioned change notes when behavior changes.
- A feature is not complete until commands, queries, DTOs, validators, repository code, EF Core configurations, migrations, authorization, Swagger, tests, architecture checks, logging, error handling, and documentation are all complete.
- Forbidden practices include business logic in controllers, role checks in controllers, direct password comparison, logging sensitive data, cross-tenant queries, `Task.Result`, `Task.Wait()`, suppressed compiler warnings without justification, secret leakage, synchronous database operations, and rich domain entities without justification.

Rationale: The service must stay debuggable, reproducible, and safe to evolve.

## Technical Constraints

- Supported stack: `.NET 8`, `ASP.NET Core Web API`, `MediatR`, `Entity Framework Core`, `SQL Server`, and JWT authentication.
- Solution layout MUST remain `Intuix.Authentication.Api`, `Intuix.Authentication.Application`, `Intuix.Authentication.Domain`, `Intuix.Authentication.Infrastructure`, and `tests/`.
- Application code MUST stay feature-based. `Auth/` and `Devices/` are current examples of valid bounded contexts.
- One class per file is required, and file names MUST match class names.
- Command and handler files MUST follow `[Command].cs` and `[Command]Handler.cs` naming.
- DTOs MUST live in `DTOs/`, validators in `Validators/`, and interfaces in `Interfaces/` within the feature folder.
- Forbidden legacy folders include `Application/Commands`, `Application/Queries`, `Application/DTOs`, `Application/Services`, and `Application/Validators`.
- Controllers MUST remain thin and delegate business logic to application handlers.
- Validation MUST complete before persistence or outbound side effects.
- Active feature specs MUST live in `.specify/specs/NNNN-module-feature/`.
- The rename catalog in `.specify/specs/README.md` is the canonical mapping for migrated specs.
- Root-level `specs/` is deprecated; do not create new feature docs there.
- Feature specs, plans, tasks, and supporting artifacts MUST stay inside their feature directory.

## Delivery Workflow

- Feature work MUST proceed `spec -> plan -> tasks -> implement`.
- `plan.md` MUST pass a Constitution Check before research starts and again after design is complete.
- `tasks.md` MUST be organized by user story and remain independently testable.
- Changes that alter behavior MUST update tests, migrations, Swagger, logs, and docs in the same delivery.
- The `NNNN` prefix MUST be a zero-padded 4-digit sequential number.
- The suffix SHOULD be a concise module/feature slug in kebab-case.
- `.specify/specs/README.md` is the migration catalog for renamed specs and the active reference for canonical naming.
- Active markdown MUST use canonical spec names only and MUST not point at retired archive locations.

## Governance

- This constitution supersedes templates, command docs, ad hoc notes, and informal conventions.
- Amendments require a written proposal, impact analysis, updates to affected artifacts, and a semantic version bump.
- Versioning policy: MAJOR for backward-incompatible governance changes, MINOR for new principles or materially expanded guidance, and PATCH for clarifications or wording fixes.
- Every plan, task list, and PR MUST be checked against this constitution before merge.
- Security, dependency, and tenant-isolation violations MUST be explicitly justified or rejected.
- Compliance review is mandatory for changes that alter behavior, dependencies, or tenant/security posture.
- `Ratified` remains the original adoption date; `Last Amended` MUST be updated whenever the constitution changes.

**Version**: 1.2.0 | **Ratified**: 2026-05-29 | **Last Amended**: 2026-07-02
