BEGIN TRAN
GO

/*
dotnet ef database drop --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

dotnet ef migrations remove --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

dotnet ef migrations add InitialAuth --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

dotnet ef database update --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

*/
--SELECT COUNT(*) FROM auth_users


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
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    user_id UNIQUEIDENTIFIER NOT NULL,

    token_hash VARBINARY(512) NOT NULL,
    expires_at DATETIME2 NOT NULL,

    revoked_at DATETIME2 NULL,

    replaced_by_token UNIQUEIDENTIFIER NULL,
    ip_address VARCHAR(45) NULL,
    user_agent NVARCHAR(300) NULL,

    FOREIGN KEY (user_id) REFERENCES auth_users(id)
);

ALTER TABLE auth_refresh_tokens ADD device VARCHAR(100);
ALTER TABLE auth_refresh_tokens ADD revocation_reason VARCHAR(250);

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







ROLLBACK
