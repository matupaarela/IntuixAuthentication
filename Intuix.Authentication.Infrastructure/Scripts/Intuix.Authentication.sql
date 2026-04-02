BEGIN TRAN
GO

/*
dotnet ef database drop --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

dotnet ef migrations remove --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

dotnet ef migrations add InitialAuth --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

dotnet ef database update --project ./Intuix.Authentication.Infrastructure --startup-project ./Intuix.Authentication.Api

*/


CREATE TABLE auth_tenants (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name NVARCHAR(150) NOT NULL,
    code NVARCHAR(50) NOT NULL UNIQUE,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE auth_organizations (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    name NVARCHAR(150) NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

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
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,

    username NVARCHAR(100) NOT NULL,
    email NVARCHAR(150) NOT NULL,
    password_hash VARBINARY(512) NOT NULL,

    is_active BIT NOT NULL DEFAULT 1,
    is_locked BIT NOT NULL DEFAULT 0,

    failed_attempts INT NOT NULL DEFAULT 0,
    last_login DATETIME2 NULL,

    created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

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

ROLLBACK
