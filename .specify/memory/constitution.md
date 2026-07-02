<!--
Sync Impact Report
Version: 1.0.0 -> 1.1.0
Modified principles:
- I. Clean Architecture Boundaries
- II. Feature-Scoped CQRS
- III. Security, Policy Authorization, and Tenant Isolation
- IV. Explicit Persistence and EF Core Discipline
- V. Testability, Observability, and Safe Change Management
Added sections:
- Technical Constraints
- Delivery Workflow
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
- .specify/templates/plan-template.md ✅ updated
- .specify/templates/spec-template.md ✅ updated
- .specify/templates/tasks-template.md ✅ updated
- .opencode/commands/speckit.constitution.md ✅ updated
- .opencode/commands/speckit.specify.md ✅ updated
- .opencode/commands/speckit.git.feature.md ✅ updated
- .opencode/commands/speckit.git.initialize.md ✅ updated
- .opencode/commands/speckit.git.validate.md ✅ updated
- .specify/extensions/git/commands/speckit.git.feature.md ✅ updated
- .specify/extensions/git/commands/speckit.git.initialize.md ✅ updated
- .specify/extensions/git/commands/speckit.git.validate.md ✅ updated
- .specify/extensions/git/README.md ✅ updated
- .specify/scripts/powershell/common.ps1 ✅ updated
- .specify/scripts/powershell/create-new-feature.ps1 ✅ updated
- .specify/extensions/git/scripts/powershell/create-new-feature.ps1 ✅ updated
- .specify/extensions/git/scripts/bash/create-new-feature.sh ✅ updated
- .specify/extensions/git/scripts/bash/git-common.sh ✅ updated
- .specify/extensions/git/scripts/powershell/git-common.ps1 ✅ updated
Follow-up TODOs: none
-->

# Intuix.Authentication Constitution

## Core Principles

### I. Clean Architecture Boundaries

- Dependency flow MUST remain `Api -> Application -> Domain`.
- `Infrastructure` MAY only implement contracts defined by `Application` or `Domain`.
- `Domain` MUST never reference `Api`, `Application`, or `Infrastructure`.
- Controllers and handlers MUST stay free of reverse-layer dependencies.

Rationale: Keeps security-critical code testable and prevents coupling across layers.

### II. Feature-Scoped CQRS

- Every feature MUST live in a feature folder and expose separate command/query paths.
- Commands and queries MUST each have exactly one handler.
- Feature code MUST keep DTOs, validators, and interfaces beside the feature.
- Business rules SHOULD live in handlers or domain services; entities remain data-centric unless a richer model is explicitly justified.
- Generic repositories, global command/query folders, and shared DTO dumps are forbidden.

Rationale: Makes changes predictable, isolated, and easier to validate independently.

### III. Security, Policy Authorization, and Tenant Isolation

- Authentication MUST use hashed passwords, hashed refresh tokens, and config-driven JWT secrets.
- Authorization MUST use policies; role checks inside controllers are forbidden.
- Every tenant-scoped entity MUST enforce tenant query filters.
- API requests MUST never accept tenant IDs from request bodies for server-side trust decisions.
- Sensitive values such as passwords, tokens, secrets, and connection strings MUST never be logged or returned to clients.

Rationale: The platform exists to protect identities and tenant boundaries; failure here is a product defect.

### IV. Explicit Persistence and EF Core Discipline

- Entity mappings MUST use `IEntityTypeConfiguration<T>` and `ApplyConfigurationsFromAssembly`.
- Repositories MUST be async, aggregate-specific, and return domain entities rather than DTOs.
- `IQueryable` MUST NOT leak from repository interfaces.
- Migrations MUST be generated through the CLI and reviewed before merge.
- Read-only queries SHOULD use `AsNoTracking`, projection, and eager loading only when required.

Rationale: Persistence rules keep data access predictable, performant, and reviewable.

### V. Testability, Observability, and Safe Change Management

- Features that touch auth, tenancy, security, or persistence MUST include unit and integration coverage.
- Architecture tests MUST verify dependency direction, folder rules, and repository constraints.
- Structured logging with correlation and tenant context is mandatory; debug logging is environment-only.
- Features MUST be implemented in the smallest safe slice and documented with versioned change notes when behavior changes.

Rationale: The service must stay debuggable, reproducible, and safe to evolve.

## Technical Constraints

- Supported stack: `.NET 8`, `ASP.NET Core Web API`, `MediatR`, `Entity Framework Core`, `SQL Server`, and JWT authentication.
- Solution layout MUST remain `Intuix.Authentication.Api`, `Intuix.Authentication.Application`,
  `Intuix.Authentication.Domain`, `Intuix.Authentication.Infrastructure`, and `tests/`.
- Application code MUST stay feature-based. `Auth/` and `Devices/` are current examples of valid bounded contexts.
- One class per file is required, and file names MUST match class names.
- Command and handler files MUST follow `[Command].cs` and `[Command]Handler.cs` naming.
- DTOs MUST live in `DTOs/`, validators in `Validators/`, and interfaces in `Interfaces/` within the feature folder.
- Forbidden legacy folders include `Application/Commands`, `Application/Queries`, `Application/DTOs`,
  `Application/Services`, and `Application/Validators`.
- Database tables MUST use `snake_case` with the `auth_` prefix; columns and JWT claims MUST use `snake_case`.
- Controllers MUST remain thin and delegate business logic to application handlers.
- Validation MUST complete before persistence or outbound side effects.
- Generic repositories, DataAnnotation persistence mapping, domain events, event sourcing, outbox pattern,
  and microservices are not part of this codebase unless explicitly re-approved.
- Access tokens MUST remain short-lived (15 minutes), signed with HMAC-SHA256, and configured externally.
- Refresh tokens MUST be cryptographically random, hashed before storage, rotated on use, and revoked on reuse.
- Tenant resolution MUST come from JWT claims and `ICurrentUser`; request bodies MUST not provide trusted tenant IDs.

## Delivery Workflow

- Active feature specs MUST live in `.specify/specs/NNNN-module-feature/`.
- The `NNNN` prefix MUST be a zero-padded 4-digit sequential number.
- The suffix SHOULD be a concise module/feature slug in kebab-case, such as `auth-login` or `tenant-management`.
- Root-level `specs/` is deprecated; do not create new feature docs there.
- `spec.md`, `plan.md`, `tasks.md`, and supporting artifacts live inside the feature directory.
- `.specify-old/specs/` is a read-only archive. Do not edit it in place.
- `.specify/specs/README.md` is the migration catalog for historical specs and the canonical index for the new naming scheme.
- Feature work MUST proceed `spec -> plan -> tasks -> implement`.
- `plan.md` MUST pass a Constitution Check before research starts and again after design is complete.
- `tasks.md` MUST be organized by user story and remain independently testable.
- Changes that alter behavior MUST update tests, migrations, Swagger, logs, and docs in the same delivery.

## Governance

- This constitution supersedes templates, command docs, ad hoc notes, and informal conventions.
- Amendments require a written proposal, impact analysis, updates to affected artifacts, and a semantic version bump.
- Versioning policy: MAJOR for backward-incompatible governance changes, MINOR for new principles or materially expanded guidance,
  and PATCH for clarifications or wording fixes.
- Every plan, task list, and PR MUST be checked against this constitution before merge.
- Security, dependency, and tenant-isolation violations MUST be explicitly justified or rejected.
- `Ratified` remains the original adoption date; `Last Amended` MUST be updated whenever the constitution changes.

**Version**: 1.1.0 | **Ratified**: 2026-05-29 | **Last Amended**: 2026-07-02
