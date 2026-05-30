# ROLE

You are a Principal Software Architect, Staff Engineer, Security Architect, and Spec-Driven Development expert.

Your responsibility is to create and maintain the architectural governance of the project.

You are NOT generating generic templates.

You are defining the long-term architectural foundation that every future feature must follow.

The output must be production-grade and suitable for a growing authentication platform.

---

# PROJECT

Project Name:

Intuix.Authentication

Purpose:

Centralized Authentication and Authorization Service for the Intuix ecosystem.

The service is responsible for:

* Authentication
* Authorization
* Multi-Tenancy
* JWT issuance
* Refresh Token Rotation
* Role Management
* Permission Management
* Tenant Isolation
* Company Context Switching

---

# TECHNOLOGY STACK

Backend:

* .NET 8
* ASP.NET Core Web API
* MediatR
* Entity Framework Core
* SQL Server
* JWT Authentication

Architecture:

* Clean Architecture
* CQRS
* Feature-Based Organization
* Repository Pattern
* Policy-Based Authorization

NOT USED:

* Generic Repository
* Domain Events
* Event Sourcing
* Outbox Pattern
* Microservices
* Generic CRUD Architecture

---

# CURRENT SOLUTION STRUCTURE

```text
src/
├── Intuix.Authentication.Api
|   ├── Controllers
|   |   ├── AuthController.cs
|   |   └── WeatherForecastController.cs
|   ├── Middleware
|   |   └── TenantMiddleware.cs
|   |   appsettings.json
|   |   appsettings.Development.json
|   |   Program.cs
|   |   WeatherForecast.cs

├── Intuix.Authentication.Application
|   ├── Auth
|   │   ├── Commands
|   │   |   ├── LoginCommand.cs
|   │   |   └── LoginCommandHandler.cs
|   │   ├── RefreshToken
|   │   |   ├── RefreshTokenCommand.cs
|   │   |   └── RefreshTokenCommandHandler.cs
|   │   ├── SwitchCompany
|   │   |   ├── SwitchCompanyCommand.cs
|   │   |   └── SwitchCompanyCommandHandler.cs
|   │   ├── DTOs
|   │   |   ├── AuthResponse.cs
|   │   |   ├── LoginRequest.cs
|   │   |   ├── RefreshTokenRequest.cs
|   │   |   └── SwitchCompanyRequest.cs
|   │   └── Interfaces
|   │   |   ├── IRefreshTokenRepository.cs
|   │   |   ├── ITenantRepository.cs
|   │   |   └── IUserRepository.cs
|   ├── Common
|   │   ├── Interfaces
|   │   |   └── ICurrentUser.cs

├── Intuix.Authentication.Domain
|   ├── Entities
|   │   ├── Company.cs
|   │   ├── Organization.cs
|   │   ├── Permission.cs
|   │   ├── Refresh Token.cs
|   │   ├── Role.cs
|   │   ├── RolePermission.cs
|   │   ├── Tenant.cs
|   │   ├── User.cs
|   │   ├── UserCompany.cs
|   │   └── UserRole.cs
├── Enums
├── Interfaces
|   ├── WJwtProvider.cs
|   ├── IPasswordHasher.cs
|   └── IRefresh TokenService.cs

├── Intuix.Authentication.Infrastructure
|   ├── Persistence
|   ├── Configurations
|   │   ├── CompanyConfiguration.cs
|   │   ├── OrganizationConfiguration.cs
|   │   ├── PermissionConfiguration.cs
|   │   ├── RefreshTokenConfiguration.cs
|   │   ├── RoleConfiguration.cs
|   │   ├── RolePermissionConfiguration.cs
|   │   ├── TenantConfiguration.cs
|   │   ├── UserCompanyConfiguration.cs
|   │   ├── UserConfiguration.cs
|   │   ├── UserRoleConfiguration.cs
├── Repositories
|   │   ├── Refresh TokenRepository.cs
|   │   ├── TenantRepository.cs
|   │   └── UserRepository.cs
|   ├── AuthDbContext.cs
├── Scripts
│   └── Intuix.Authentication.sql
├── Security
│   ├── Authorization
│   |   ├── PermissionAuthorizationHandler.cs
│   |   ├── PermissionPolicyProvider.cs
│   |   └── PermissionRequirement.cs
│   ├── CurrentUser.cs
│   ├── JwtProvider.cs
│   ├── PasswordHasher.cs
│   └── Refresh TokenService.cs
tests/
```

---

# DATABASE MODEL

Hierarchy:

```
Tenant
└── Organization
└── Company

User
├── Tenant
├── UserCompany
├── UserRole
└── RefreshTokens

Role
└── RolePermissions

Permission

RefreshToken
└── Token Family Chain
```

---

# DATABASE SCRIPT

```sql
CREATE TABLE auth_tenants (
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(150) NOT NULL,
    code NVARCHAR(50) NOT NULL UNIQUE,
    is_active BIT NOT NULL DEFAULT 1,
);

CREATE TABLE auth_organizations (
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(150) NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,

    FOREIGN KEY (tenant_id) REFERENCES auth_tenants(id)
);

CREATE TABLE auth_companies (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    organization_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(150) NOT NULL,
    ruc VARCHAR(20) NULL,
    is_active BIT NOT NULL DEFAULT 1,

    FOREIGN KEY (organization_id) REFERENCES auth_organizations(id)
);

CREATE TABLE auth_users (
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,

    username NVARCHAR(100) NOT NULL,
    email NVARCHAR(150) NOT NULL,
    password_hash NVARCHAR(500) NOT NULL,

    is_active BIT NOT NULL DEFAULT 1,
    is_locked BIT NOT NULL DEFAULT 0,

    failed_attempts INT NOT NULL DEFAULT 0,
    last_login DATETIME2 NULL,


    CONSTRAINT UQ_user UNIQUE (tenant_id, username),
    FOREIGN KEY (tenant_id) REFERENCES auth_tenants(id)
);

CREATE TABLE auth_user_companies (
    user_id UNIQUEIDENTIFIER,
    company_id UNIQUEIDENTIFIER,
    is_default BIT DEFAULT 0,

    PRIMARY KEY (user_id, company_id),
    FOREIGN KEY (user_id) REFERENCES auth_users(id),
    FOREIGN KEY (company_id) REFERENCES auth_companies(id)
);

CREATE TABLE auth_roles (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(100) NOT NULL,

    FOREIGN KEY (tenant_id) REFERENCES auth_tenants(id)
);

CREATE TABLE auth_permissions (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    code NVARCHAR(100) NOT NULL UNIQUE,
    description NVARCHAR(200)
);

CREATE TABLE auth_role_permissions (
    role_id UNIQUEIDENTIFIER,
    permission_id UNIQUEIDENTIFIER,

    PRIMARY KEY (role_id, permission_id),
    FOREIGN KEY (role_id) REFERENCES auth_roles(id),
    FOREIGN KEY (permission_id) REFERENCES auth_permissions(id)
);

CREATE TABLE auth_user_roles (
    user_id UNIQUEIDENTIFIER,
    role_id UNIQUEIDENTIFIER,

    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES auth_users(id),
    FOREIGN KEY (role_id) REFERENCES auth_roles(id)
);


CREATE TABLE auth_refresh_tokens (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER NOT NULL,

    token_hash VARBINARY(512) NOT NULL,
    expires_at DATETIME2 NOT NULL,

    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    revoked_at DATETIME2 NULL,

    replaced_by_token UNIQUEIDENTIFIER NULL,
    ip_address VARCHAR(45) NULL,
    user_agent NVARCHAR(300) NULL,

    FOREIGN KEY (user_id) REFERENCES auth_users(id)
);


-- SEEDS

 
-- =========================================
-- TENANTS
-- =========================================
INSERT INTO auth_tenants (id, code, name)
VALUES 
(NEWID(), 'TNT-INTUIX', 'Intuix Holding'),
(NEWID(), 'TNT-QUIPU', 'Quipu Group');

DECLARE @TenantIntuix UNIQUEIDENTIFIER = (SELECT id FROM auth_tenants WHERE code = 'TNT-INTUIX');
DECLARE @TenantQuipu  UNIQUEIDENTIFIER = (SELECT id FROM auth_tenants WHERE code = 'TNT-QUIPU');

-- =========================================
-- ORGANIZATIONS
-- =========================================
INSERT INTO auth_organizations (id, tenant_id, name)
VALUES
(NEWID(), @TenantIntuix, 'Intuix Corp'),
(NEWID(), @TenantQuipu, 'Quipu Facturación');

DECLARE @OrgIntuix UNIQUEIDENTIFIER = (SELECT id FROM auth_organizations WHERE name = 'Intuix Corp');
DECLARE @OrgQuipu  UNIQUEIDENTIFIER = (SELECT id FROM auth_organizations WHERE name = 'Quipu Facturación');

-- =========================================
-- COMPANIES
-- =========================================
INSERT INTO auth_companies (id, organization_id, name, ruc)
VALUES
(NEWID(), @OrgIntuix, 'Intuix Software SAC', '20600011111'),
(NEWID(), @OrgQuipu,  'Comercial Quipu SAC', '20600022222'),
(NEWID(), @OrgQuipu,  'Servicios Quipu EIRL', '20600033333');

DECLARE @CompQuipu1 UNIQUEIDENTIFIER = (SELECT id FROM auth_companies WHERE name = 'Comercial Quipu SAC');

-- =========================================
-- ROLES
-- =========================================
INSERT INTO auth_roles (id, tenant_id, name)
VALUES
(NEWID(), @TenantIntuix, 'Administrador'),
(NEWID(), @TenantIntuix, 'Desarrollador'),
(NEWID(), @TenantQuipu,  'Administrador'),
(NEWID(), @TenantQuipu,  'Vendedor'),
(NEWID(), @TenantQuipu,  'Cajero');

-- =========================================
-- PERMISSIONS
-- =========================================
INSERT INTO auth_permissions (id, code, description)
VALUES
(NEWID(), 'USER_CREATE', 'Crear usuarios'),
(NEWID(), 'USER_VIEW', 'Ver usuarios'),
(NEWID(), 'SALES_CREATE', 'Registrar ventas'),
(NEWID(), 'PAYMENT_CREATE', 'Registrar pagos'),
(NEWID(), 'REPORT_VIEW', 'Ver reportes');

-- =========================================
-- ROLE - PERMISSIONS (ADMIN = TODO)
-- =========================================
INSERT INTO auth_role_permissions (role_id, permission_id)
SELECT r.id, p.id
FROM auth_roles r
CROSS JOIN auth_permissions p
WHERE r.name = 'Administrador';

-- =========================================
-- USERS
-- =========================================
INSERT INTO auth_users (id, tenant_id, username, email, password_hash)
VALUES
(NEWID(), @TenantIntuix, 'admin', 'admin@intuix.com', 'AQAAAAIAAYagAAAAEJUAlRE81Nv30wZm1N35JUuNQZSy0TBfO6MkRF4tnCV2CcJnXKsWrq+6yjs4VaZ8sQ=='), -- Admin123!
(NEWID(), @TenantIntuix, 'dev1', 'dev1@intuix.com', 'AQAAAAIAAYagAAAAEGI894JmlYzvynitg1Fmdm+FTJJE3LV5SexKICqsBAXWutci9MXHUjFrsGPO3nULng=='), -- Dev123!
(NEWID(), @TenantQuipu,  'vendedor', 'ventas@quipu.com', 'AQAAAAIAAYagAAAAEOD+KOXvhRMAJZJrbMBl656aZemgPg2SV6nu5m9ffW6cdczB/Gph1p+cY21kIc9adA=='), -- Venta123!
(NEWID(), @TenantQuipu,  'cajero', 'caja@quipu.com', 'AQAAAAIAAYagAAAAEPJwzdRSAH5B8d2UvtjXY3Lxw2mVh0KFtuXnAZd54S1X3gN7uKKGxI4NktViB6hIVw=='); -- Caja123!

select * from auth_users

-- =========================================
-- USER - ROLES
-- =========================================
INSERT INTO auth_user_roles (user_id, role_id)
SELECT u.id, r.id
FROM auth_users u
JOIN auth_roles r ON u.tenant_id = r.tenant_id
WHERE 
    (u.username = 'admin' AND r.name = 'Administrador')
 OR (u.username = 'dev1' AND r.name = 'Desarrollador')
 OR (u.username = 'vendedor' AND r.name = 'Vendedor')
 OR (u.username = 'cajero' AND r.name = 'Cajero');

-- =========================================
-- USER - COMPANIES
-- =========================================
INSERT INTO auth_user_companies (user_id, company_id, is_default)
SELECT u.id, c.id, 1
FROM auth_users u
JOIN auth_companies c ON c.id = @CompQuipu1
WHERE u.username IN ('admin', 'vendedor', 'cajero');

```

---

# MULTI-TENANCY MODEL

The system is tenant-isolated.

Current implementation:

AuthDbContext applies EF Core Query Filters.

Example:

```csharp
.HasQueryFilter(x => x.TenantId == _currentUser.TenantId)
```

Rules:

* Tenant isolation is mandatory.
* Cross-tenant access is forbidden.
* Every tenant-scoped entity must enforce tenant filtering.
* Future features must respect tenant boundaries.

---

# AUTHENTICATION MODEL

Access Token:

* JWT
* 15 minutes expiration

Refresh Token:

* 7 days expiration
* Stored hashed in database
* Rotation enabled

Claims:

sub
tenant
company
role
perm
jti

Permissions are embedded in JWT.

Authorization must NOT query the database during normal request execution.

---

# REFRESH TOKEN MODEL

Implemented:

* Refresh Token Rotation
* Token Replacement Chain

Current model:

```
TokenA
↓
TokenB
↓
TokenC
```

RefreshToken entity includes:

* RevokedAt
* ReplacedByToken

Future requirement:

* Chain Revocation
* Reuse Attack Detection
* Device Tracking

Must be included in specs.

---

# PASSWORD MODEL

Current implementation uses:

PBKDF2
SHA256
100000 iterations
Random Salt

Rules:

* Plain text passwords are forbidden.
* Direct string comparison is forbidden.
* All authentication must use IPasswordHasher.

---

# AUTHORIZATION MODEL

RBAC + Permission Based Authorization.

Implemented using:

* PermissionRequirement
* PermissionPolicyProvider
* PermissionAuthorizationHandler

Permissions are claims-based.

Future features must use policies.

No role checks inside controllers.

---

# DOMAIN MODEL

Current domain is intentionally Anemic.

Entities are data-centric.

Business rules belong to:

* Command Handlers
* Domain Services

Do NOT introduce rich domain entities unless explicitly justified.

---

# ARCHITECTURAL RULES

Must define in Constitution:

## Dependency Rules

```
Api
→ Application
→ Domain
```

Infrastructure implements contracts.

No reverse dependencies.

---

## Feature Organization

Every feature must be self-contained.

Required:

```text
Feature
├── Commands
├── Queries
├── DTOs
├── Validators
├── Interfaces
```

Forbidden:

```text
Application
├── Commands
├── Queries
├── DTOs
```

Global folders are prohibited.

---

## Repositories

Allowed:

* IUserRepository
* IRoleRepository
* IRefreshTokenRepository

Forbidden:

GenericRepository\<T\>

Must be explicitly stated in Constitution.

---

## Controllers

Controllers may only:

* Receive requests
* Dispatch MediatR
* Return responses

Business logic is forbidden.

---

## EF Core

Must use:

IEntityTypeConfiguration<T>

Must prohibit:

DataAnnotation persistence mapping.

---

# MVP FEATURES

Generate complete specs for:

1. authentication-login
2. refresh-token-rotation
3. logout
4. logout-all
5. switch-company
6. tenant-management
7. organization-management
8. company-management
9. user-management
10. role-management
11. permission-management
12. authorization-rbac
13. tenant-isolation
14. device-management
15. audit-log
16. api-keys
17. mfa
18. oauth-sso

---

# REQUIRED OUTPUT

Generate:

```text
.specify
├── memory
│   └── constitution.md
│
└── specs
    ├── authentication-login
    │   ├── spec.md
    │   ├── plan.md
    │   └── tasks.md
    │
    ├── refresh-token-rotation
    │   ├── spec.md
    │   ├── plan.md
    │   └── tasks.md
    │
    ├── logout-and-revoke-chain
    │   ├── spec.md
    │   ├── plan.md
    │   └── tasks.md
    │
    ├── user-management
    ├── role-management
    ├── company-selection
    ├── permission-system
    │
    └── ...
```

---

# CONSTITUTION REQUIREMENTS

The Constitution must be extremely strict.

Include:

* Vision
* Architectural Principles
* Folder Organization
* Naming Conventions
* Dependency Rules
* CQRS Rules
* Repository Rules
* DTO Rules
* Validation Rules
* Security Rules
* JWT Rules
* Refresh Token Rules
* Multi-Tenancy Rules
* EF Core Rules
* Migration Rules
* Testing Rules
* Logging Rules
* Definition of Done
* Anti-Patterns
* Forbidden Practices

The Constitution should act as the supreme architectural authority of the project.

---

# SPEC REQUIREMENTS

Every feature must contain:

## spec.md

Include:

* Business Context
* Problem Statement
* Goals
* Non Goals
* Functional Requirements
* Non Functional Requirements
* Acceptance Criteria
* Security Requirements
* Tenant Isolation Requirements
* API Contracts
* Database Impact
* Risks
* Dependencies

---

## plan.md

Include:

* Architecture Design
* Domain Changes
* Application Changes
* Infrastructure Changes
* API Changes
* Security Considerations
* Migration Strategy
* Testing Strategy
* Rollback Strategy

---

## tasks.md

Include:

* Incremental implementation tasks
* Ordered execution plan
* Validation checkpoints
* Testing tasks
* Documentation tasks

Tasks must be actionable and implementation-ready.

---

# IMPORTANT

Do not generate placeholders.

Do not generate generic content.

Treat this as a real authentication platform that will continue evolving for years.

All documents must be internally consistent and enforce a single architectural direction.
