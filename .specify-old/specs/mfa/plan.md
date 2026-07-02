# mfa - Implementation Plan

## Architecture Design

```
MfaController
  → POST /api/mfa/enable → MfaEnableCommand
  → POST /api/mfa/verify-setup → MfaVerifySetupCommand
  → POST /api/mfa/disable → MfaDisableCommand
  → GET /api/mfa/status → MfaGetStatusQuery
```

## Domain Changes

### 1. Add MFA Fields to User Entity

**File:** `Domain/Entities/User.cs`

```csharp
public bool MfaEnabled { get; set; }
public byte[]? MfaSecret { get; set; }
public string? BackupCodes { get; set; }
```

## Application Changes

### 1. Create Feature Folder

```
Application/Mfa/
├── Commands/
│   ├── MfaEnableCommand.cs/.Handler.cs
│   ├── MfaVerifySetupCommand.cs/.Handler.cs
│   └── MfaDisableCommand.cs/.Handler.cs
├── Queries/
│   └── MfaGetStatusQuery.cs/.Handler.cs
├── DTOs/
│   ├── MfaEnableResponse.cs
│   └── MfaStatusResponse.cs
├── Validators/
└── Interfaces/
    └── IMfaService.cs
```

### 2. Create IMfaService

```csharp
public interface IMfaService
{
    string GenerateSecret();
    string GetQrCodeUrl(string email, string secret);
    bool VerifyTotp(string secret, string code);
    string[] GenerateBackupCodes();
    string EncryptSecret(string secret);
    string DecryptSecret(byte[] encrypted);
}
```

## Infrastructure Changes

### 1. Create MfaService

**File:** `Infrastructure/Security/MfaService.cs`

Implement TOTP using HMAC-SHA1.

### 2. Update UserConfiguration

Add new columns.

### 3. Create Migration

Add MFA columns to auth_users.

## API Changes

### 1. Create MfaController

Endpoints with `[Authorize]`.

## Security Considerations

1. TOTP secrets encrypted at rest
2. Backup codes hashed
3. TOTP verification uses constant-time comparison

## Migration Strategy

1. Add nullable columns
2. Backfill if needed

## Testing Strategy

1. Unit tests for TOTP generation/verification
2. Integration tests for MFA flow

## Rollback Strategy

1. Remove new files
2. Drop migration
