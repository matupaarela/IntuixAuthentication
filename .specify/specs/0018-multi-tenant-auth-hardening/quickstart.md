# Quickstart: Multi-Tenant Auth Hardening

## Prerequisites

- .NET 8 SDK
- SQL Server or LocalDB connection configured for the API project
- A JWT key, issuer, and audience configured for the local environment

## Local Verification

1. Restore and build the solution.
2. Apply the database changes for the auth schema.
3. Run the test suite.

```bash
dotnet test IntuixAuthentication.sln
```

4. Start the API project.

```bash
dotnet run --project Intuix.Authentication.Api
```

5. Open Swagger in development and validate the auth flows in this order:
- `POST /auth/login`
- `POST /auth/refresh`
- `POST /auth/switch-company`
- `GET /api/devices`
- `DELETE /api/devices/{tokenId}`
- `POST /api/devices/revoke-all`

## Expected Behaviors

- Login returns an access token, refresh token, and tenant/company context.
- Failed logins increase the failure counter and eventually lock the account.
- Refresh rotates the session and rejects reused tokens with a generic security error.
- Company switching only succeeds for companies assigned to the current user.
- Device listing shows active sessions and marks the current one.

## Regression Checklist

- Cross-tenant access is denied.
- Locked accounts stay locked until manual release.
- Permission-protected endpoints reject missing permissions.
- Session revocation removes only the intended session family.
