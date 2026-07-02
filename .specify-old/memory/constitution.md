# Intuix.Authentication - Architectural Constitution

**Version:** 1.0.0
**Status:** Active
**Last Updated:** 2026-05-29

---

## 1. Vision

Intuix.Authentication is the centralized Authentication and Authorization service for the Intuix ecosystem. It provides secure, multi-tenant identity management, JWT issuance, refresh token rotation, RBAC-based authorization, and tenant isolation. This constitution is the supreme architectural authority — all future development must comply with it.

---

## 2. Architectural Principles

### 2.1 Clean Architecture

The solution follows strict Clean Architecture with unidirectional dependency flow:

```
Api → Application → Domain
```

Infrastructure implements contracts defined in Application and Domain. No reverse dependencies are permitted under any circumstance.

### 2.2 CQRS

All operations are modeled as Commands (write) or Queries (read). Commands mutate state and return minimal results. Queries are read-only and may return DTOs directly. Commands and Queries live inside their respective feature folders.

### 2.3 Feature-Based Organization

Every feature is self-contained. Cross-feature sharing is limited to shared interfaces in `Common/Interfaces`. Features are organized as:

```
Feature/
├── Commands/
│   └── FeatureCommand.cs
│   └── FeatureCommandHandler.cs
├── Queries/
│   └── FeatureQuery.cs
│   └── FeatureQueryHandler.cs
├── DTOs/
│   └── FeatureRequest.cs
│   └── FeatureResponse.cs
├── Validators/
│   └── FeatureValidator.cs
└── Interfaces/
    └── IFeatureRepository.cs
```

### 2.4 Repository Pattern

Each aggregate root has a dedicated repository interface. Generic repositories are strictly forbidden.

### 2.5 Policy-Based Authorization

All authorization is claims-based using policies. Role checks inside controllers are forbidden. Authorization is enforced via `[Authorize(Policy = "...")]` attributes.

### 2.6 Anemic Domain Model

Entities are data-centric. Business rules belong in Command Handlers or Domain Services. Rich domain entities are not introduced unless explicitly justified.

---

## 3. Folder Organization

### 3.1 Solution Structure

```
src/
├── Intuix.Authentication.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── Intuix.Authentication.Application/
│   ├── Auth/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Interfaces/
│   ├── Common/
│   │   └── Interfaces/
│   └── [Feature]/
│       ├── Commands/
│       ├── Queries/
│       ├── DTOs/
│       ├── Validators/
│       └── Interfaces/
│
├── Intuix.Authentication.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
│
├── Intuix.Authentication.Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── Repositories/
│   ├── Security/
│   │   ├── Authorization/
│   │   ├── JwtProvider.cs
│   │   ├── PasswordHasher.cs
│   │   ├── RefreshTokenService.cs
│   │   └── CurrentUser.cs
│   └── AuthDbContext.cs
│
└── tests/
```

### 3.2 Forbidden Folder Structures

```
Application/
├── Commands/          ← FORBIDDEN (global commands)
├── Queries/           ← FORBIDDEN (global queries)
├── DTOs/              ← FORBIDDEN (global DTOs)
├── Services/          ← FORBIDDEN (global services)
└── Validators/        ← FORBIDDEN (global validators)
```

---

## 4. Naming Conventions

### 4.1 General

| Element | Convention | Example |
|---------|-----------|---------|
| Projects | PascalCase, dot-separated | `Intuix.Authentication.Api` |
| Classes | PascalCase | `LoginCommandHandler` |
| Interfaces | PascalCase, `I` prefix | `IUserRepository` |
| Methods | PascalCase | `GetByUsernameAsync` |
| Properties | PascalCase | `TenantId` |
| Parameters | camelCase | `userId` |
| Private fields | camelCase, `_` prefix | `_userRepository` |
| Constants | PascalCase | `MaxFailedAttempts` |
| Database tables | `snake_case` with prefix | `auth_users` |
| Database columns | `snake_case` | `password_hash` |
| JWT claims | `snake_case` | `tenant_id` |

### 4.2 Command/Query Naming

| Pattern | Naming | Example |
|---------|--------|---------|
| Create command | `[Entity]CreateCommand` | `UserCreateCommand` |
| Update command | `[Entity]UpdateCommand` | `UserUpdateCommand` |
| Delete command | `[Entity]DeleteCommand` | `UserDeleteCommand` |
| Login command | `LoginCommand` | `LoginCommand` |
| Query by ID | `[Entity]GetByIdQuery` | `UserGetByIdQuery` |
| Query list | `[Entity]GetListQuery` | `UserGetListQuery` |

### 4.3 File Naming

- One class per file
- File name matches class name
- Command handler files: `[Command].cs` and `[Command]Handler.cs` in same folder

---

## 5. Dependency Rules

### 5.1 Allowed Dependencies

```
Api → Application (commands, DTOs, interfaces)
Api → Infrastructure (DI registration only)
Application → Domain (entities, interfaces)
Infrastructure → Application (implements interfaces)
Infrastructure → Domain (implements interfaces)
```

### 5.2 Forbidden Dependencies

- Domain → Application
- Domain → Infrastructure
- Domain → Api
- Application → Infrastructure
- Application → Api

### 5.3 DI Registration

All DI registration happens in `Program.cs` of the Api project. Infrastructure implementations are registered against Application/Domain interfaces.

---

## 6. CQRS Rules

### 6.1 Commands

- Must implement `IRequest<TResponse>` from MediatR
- Must have exactly one handler implementing `IRequestHandler<TCommand, TResponse>`
- Commands are mutable objects with setters
- Commands must not return collections for write operations
- Validation must be done in handler or FluentValidation validator

### 6.2 Queries

- Must implement `IRequest<TResponse>` from MediatR
- Must have exactly one handler implementing `IRequestHandler<TQuery, TResponse>`
- Queries must not mutate state
- Queries may return DTOs directly (no mapping required for read-only)

### 6.3 Handlers

- Handlers must not contain HTTP concerns
- Handlers must not access `HttpContext` directly
- Handlers must use constructor injection only
- Handlers must be stateless
- One handler per command/query

---

## 7. Repository Rules

### 7.1 Allowed Repositories

| Repository | Aggregate Root | Location |
|-----------|---------------|----------|
| `IUserRepository` | User | Application/Auth/Interfaces |
| `IRoleRepository` | Role | Application/[Feature]/Interfaces |
| `IRefreshTokenRepository` | RefreshToken | Application/Auth/Interfaces |
| `ITenantRepository` | Tenant | Application/Auth/Interfaces |
| `IOrganizationRepository` | Organization | Application/[Feature]/Interfaces |
| `ICompanyRepository` | Company | Application/[Feature]/Interfaces |
| `IPermissionRepository` | Permission | Application/[Feature]/Interfaces |

### 7.2 Forbidden Repositories

- `GenericRepository<T>`
- `IRepository<T>`
- `IBaseRepository<T>`
- Any generic CRUD repository

### 7.3 Repository Interface Rules

- Interfaces live in Application layer (feature-specific or Auth/Interfaces)
- Methods must be async (`Task<T>`)
- Methods must accept `CancellationToken`
- No `IQueryable` leakage from repository interfaces
- Repositories return domain entities, not DTOs

### 7.4 Repository Implementation Rules

- Implementations live in Infrastructure/Persistence/Repositories
- Must use `AuthDbContext` via constructor injection
- Must respect tenant query filters (enforced by EF Core)
- Must use `SaveChangesAsync` for persistence

---

## 8. DTO Rules

### 8.1 Request DTOs

- Must be records or classes with public setters
- Must not contain business logic
- Must not contain navigation properties
- Naming: `[Feature]Request` or `[Command]Request`

### 8.2 Response DTOs

- Must be records or classes with public setters
- Must not expose domain entities directly (except for simple cases)
- Naming: `[Feature]Response` or `[Command]Response`

### 8.3 DTO Location

- DTOs must live in the feature folder: `Feature/DTOs/`
- Shared DTOs (like `AuthResponse`) may live in `Auth/DTOs/`
- DTOs must not be placed in Domain or Infrastructure layers

---

## 9. Validation Rules

### 9.1 Validation Approach

- Use FluentValidation for complex validation rules
- Simple validation (null checks, format) may be done in handlers
- Validation must occur before any database operation

### 9.2 Validator Location

- Validators live in `Feature/Validators/`
- One validator per command/query
- Naming: `[Command]Validator` or `[Query]Validator`

### 9.3 Validation Rules

- Must validate all required fields
- Must validate string lengths
- Must validate GUID format
- Must validate email format where applicable
- Must return structured error responses

---

## 10. Security Rules

### 10.1 Password Handling

- Plain text passwords are strictly forbidden
- Direct string comparison is strictly forbidden
- All authentication must use `IPasswordHasher`
- Passwords must be hashed with PBKDF2, SHA256, 100,000 iterations, random salt
- Password hashes must never be logged

### 10.2 Token Security

- JWT secrets must never be hardcoded in source code
- JWT secrets must come from configuration (environment variables, secret manager)
- Refresh tokens must be stored hashed in database
- Token values must never be logged
- Token hashes must never be logged

### 10.3 Input Sanitization

- All user input must be validated before processing
- SQL injection is prevented by EF Core parameterized queries
- XSS is prevented by API response encoding

### 10.4 Error Handling

- Authentication errors must return generic messages ("Invalid credentials")
- Detailed error messages must never be returned to clients
- Stack traces must never be exposed

---

## 11. JWT Rules

### 11.1 Token Structure

| Claim | Source | Description |
|-------|--------|-------------|
| `sub` | User.Id | User identifier |
| `tenant` | User.TenantId | Tenant identifier |
| `company` | UserCompany (default) | Active company |
| `role` | UserRole.Name | User roles (multiple) |
| `perm` | RolePermission.Code | User permissions (multiple) |
| `jti` | Random GUID | Unique token identifier |

### 11.2 Token Configuration

| Parameter | Value |
|-----------|-------|
| Algorithm | HMAC-SHA256 |
| Access Token Expiry | 15 minutes |
| Issuer | Configured via `Jwt:Issuer` |
| Audience | Configured via `Jwt:Audience` |

### 11.3 Token Rules

- Access tokens must be short-lived (15 minutes)
- Permissions are embedded in JWT claims
- Authorization must NOT query the database during normal request execution
- Token blacklist is not implemented (use short expiry + refresh rotation)

---

## 12. Refresh Token Rules

### 12.1 Token Generation

- Refresh tokens must be cryptographically random (64 bytes minimum)
- Tokens must be hashed with SHA256 before storage
- Only the hash is stored in the database
- The raw token is returned to the client once

### 12.2 Token Rotation

- Every refresh operation must revoke the old token and issue a new one
- The `ReplacedByToken` field links old token to new token
- Rotation creates a chain: TokenA → TokenB → TokenC

### 12.3 Token Lifecycle

| State | Condition |
|-------|-----------|
| Active | `RevokedAt` is null, `ExpiresAt` > now |
| Expired | `ExpiresAt` <= now |
| Revoked | `RevokedAt` is not null |
| Reused | Token received but `RevokedAt` is not null (reuse attack) |

### 12.4 Security

- Reuse of a revoked token must trigger chain revocation
- All tokens in the family must be revoked on reuse detection
- Refresh tokens expire after 7 days
- Device metadata (IP, User-Agent) must be captured

---

## 13. Multi-Tenancy Rules

### 13.1 Isolation

- Tenant isolation is mandatory for all tenant-scoped entities
- Cross-tenant access is strictly forbidden
- Every tenant-scoped entity must enforce query filters

### 13.2 Query Filters

```csharp
.HasQueryFilter(e => e.TenantId == _currentUser.TenantId)
```

### 13.3 Tenant-Scoped Entities

| Entity | Tenant Field |
|--------|-------------|
| User | `TenantId` |
| Role | `TenantId` |
| Organization | `TenantId` |
| Company | Via Organization |
| RefreshToken | Via User |

### 13.4 Tenant Resolution

1. JWT `tenant` claim is the primary source
2. `ICurrentUser.TenantId` provides the resolved value
3. Query filters use `ICurrentUser.TenantId`
4. Manual override via `ICurrentUser.SetTenant()` for pre-authentication flows

### 13.5 Rules

- `SetTenant()` may only be called during authentication flows
- Every new entity creation must set `TenantId` from `ICurrentUser.TenantId`
- Cross-tenant queries are forbidden even with direct SQL
- API endpoints must never accept tenant ID from request body

---

## 14. EF Core Rules

### 14.1 Configuration

- Must use `IEntityTypeConfiguration<T>` for all entity mappings
- Must NOT use DataAnnotation persistence mapping
- Configurations must be in `Persistence/Configurations/`
- Must use `ApplyConfigurationsFromAssembly()` in `AuthDbContext`

### 14.2 Query Filters

- All tenant-scoped entities must have query filters
- Query filters must reference `ICurrentUser.TenantId`
- Disabled entities should have separate query filters where needed

### 14.3 Migrations

- Migrations must be generated via CLI: `dotnet ef migrations add [Name]`
- Migration names must be PascalCase and descriptive
- Migrations must be reviewed before application
- Seed data must be in separate migration or SQL scripts

### 14.4 Performance

- Must use `AsNoTracking()` for read-only queries
- Must use `Include()` for required related data
- Must use `projection` (Select) for DTO queries
- Must avoid N+1 query patterns

---

## 15. Migration Rules

### 15.1 Migration Naming

- Pattern: `[Timestamp]_[DescriptiveName]`
- Example: `20260529_AddAuditLogTable`

### 15.2 Migration Process

1. Generate migration via CLI
2. Review generated SQL
3. Test against development database
4. Commit migration file to source control
5. Apply in deployment pipeline

### 15.3 Seed Data

- Seed data for development goes in SQL scripts
- Seed data for production must be in migrations
- Seed data must respect tenant boundaries

---

## 16. Testing Rules

### 16.1 Unit Tests

- Must test command handlers in isolation
- Must mock all repository interfaces
- Must test validation logic
- Must test error conditions
- Test naming: `[Method]_[Scenario]_[ExpectedResult]`

### 16.2 Integration Tests

- Must test API endpoints end-to-end
- Must test database operations
- Must test authentication flows
- Must test authorization policies
- Must test tenant isolation

### 16.3 Test Organization

```
tests/
├── Intuix.Authentication.UnitTests/
│   ├── Auth/
│   │   ├── Commands/
│   │   └── Queries/
│   └── Common/
├── Intuix.Authentication.IntegrationTests/
│   ├── Api/
│   └── Persistence/
└── Intuix.Authentication.ArchitectureTests/
    └── ArchitectureTests.cs
```

### 16.4 Architecture Tests

- Must verify dependency rules (no reverse dependencies)
- Must verify naming conventions
- Must verify folder organization
- Must verify no generic repositories
- Must verify no business logic in controllers

---

## 17. Logging Rules

### 17.1 What to Log

| Level | Events |
|-------|--------|
| Information | Login success, logout, company switch, CRUD operations |
| Warning | Failed login attempt, account lockout, token reuse detected |
| Error | Unhandled exceptions, database failures, external service failures |
| Debug | Request/response details (development only) |

### 17.2 What NOT to Log

- Passwords (plain text or hashed)
- Refresh tokens (raw or hashed)
- JWT secrets
- Connection strings
- PII beyond user ID

### 17.3 Structured Logging

- Use Serilog or built-in ILogger
- Use structured logging with named properties
- Include correlation IDs in log entries
- Include tenant ID in log entries where applicable

---

## 18. Definition of Done

A feature is considered complete when:

- [ ] All commands and queries are implemented
- [ ] All DTOs are defined
- [ ] All validators are implemented
- [ ] All repository interfaces and implementations are complete
- [ ] EF Core configurations are complete
- [ ] Migrations are generated and tested
- [ ] Authorization policies are applied
- [ ] API endpoints are documented in Swagger
- [ ] Unit tests are written for all handlers
- [ ] Integration tests are written for all endpoints
- [ ] Architecture tests pass
- [ ] Code follows naming conventions
- [ ] No security vulnerabilities identified
- [ ] No tenant isolation breaches
- [ ] Logging is implemented
- [ ] Error handling is complete
- [ ] Documentation is updated

---

## 19. Anti-Patterns

### 19.1 Forbidden Patterns

| Anti-Pattern | Why |
|-------------|-----|
| `GenericRepository<T>` | Encourages anemic design, no aggregate awareness |
| Business logic in controllers | Violates SRP, untestable |
| Role checks in controllers | Must use policy-based authorization |
| Direct password comparison | Security vulnerability |
| Logging sensitive data | Security vulnerability |
| Cross-tenant queries | Violates tenant isolation |
| Rich domain entities (without justification) | Unnecessary complexity |
| Domain events | Explicitly not used in this project |
| Event sourcing | Explicitly not used in this project |
| Outbox pattern | Explicitly not used in this project |
| Microservices | Explicitly not used in this project |
| DataAnnotation persistence mapping | Must use FluentConfiguration |

### 19.2 Code Smells to Watch

- Handlers with more than 50 lines
- Controllers with more than 3 injected dependencies
- Methods with more than 4 parameters
- Nested if/else deeper than 2 levels
- Repository with more than 10 methods
- DTOs with more than 15 properties

---

## 20. Forbidden Practices

1. **Never** store plain text passwords
2. **Never** compare passwords directly
3. **Never** log sensitive data
4. **Never** expose stack traces to clients
5. **Never** bypass tenant query filters
6. **Never** accept tenant ID from request body
7. **Never** use `GenericRepository<T>`
8. **Never** put business logic in controllers
9. **Never** use role checks in controllers
10. **Never** use DataAnnotation for persistence mapping
11. **Never** create cross-layer dependencies
12. **Never** use `Task.Result` or `Task.Wait()` in async code
13. **Never** suppress compiler warnings without justification
14. **Never** commit secrets to source control
15. **Never** use synchronous database operations

---

## 21. Amendment Process

Amendments to this constitution require:

1. Proposal with justification
2. Review by team lead
3. Impact analysis on existing features
4. Version bump following semantic versioning
5. Update of all affected specs and plans

---

**This constitution is the supreme architectural authority. All development must comply with it. Any deviation must be explicitly justified and documented.**
