# 0004-auth-mfa - Specification

## Business Context

Multi-Factor Authentication (MFA) adds an extra layer of security. Users authenticate with password (something they know) and a TOTP code (something they have). MFA is optional per user and can be enforced by tenant policy.

## Problem Statement

No MFA exists. All authentication relies solely on password. MFA using TOTP (Time-based One-Time Password) provides compatible 2FA with authenticator apps (Google Authenticator, Authy, etc.).

## Goals

1. Enable MFA setup for users (generate TOTP secret)
2. Verify TOTP code during setup
3. Require TOTP code during login when MFA is enabled
4. Allow users to disable MFA (with current password verification)
5. Generate backup codes for account recovery

## Non-Goals

- SMS-based MFA
- Email-based MFA
- Hardware security keys
- Push notification MFA

## Functional Requirements

1. **FR-01**: User can enable MFA by generating TOTP secret
2. **FR-02**: System returns TOTP secret and QR code URL
3. **FR-03**: User must verify TOTP code to complete setup
4. **FR-04**: Login with MFA requires password + TOTP code
5. **FR-05**: User can disable MFA with current password verification
6. **FR-06**: System generates 10 backup codes on MFA enable
7. **FR-07**: Backup codes can be used as TOTP alternative
8. **FR-08**: MFA status is visible in user profile

## Non-Functional Requirements

1. **NFR-01**: TOTP uses 30-second intervals
2. **NFR-02**: TOTP uses 6-digit codes
3. **NFR-03**: Backup codes are 8 characters, alphanumeric
4. **NFR-04**: TOTP secrets are stored encrypted

## Acceptance Criteria

- [ ] POST /api/0004-auth-mfa/enable returns TOTP secret and QR URL
- [ ] POST /api/0004-auth-mfa/verify-setup verifies TOTP code and activates MFA
- [ ] POST /api/0004-auth-mfa/disable disables MFA (requires password)
- [ ] Login with MFA requires TOTP code
- [ ] Backup codes work as TOTP alternative
- [ ] MFA status visible in user profile
- [ ] Invalid TOTP code rejected

## API Contract

### POST /api/0004-auth-mfa/enable

**Headers:** `Authorization: Bearer {accessToken}`

**Response (200):**
```json
{
  "secret": "string (base32)",
  "qrCodeUrl": "otpauth://totp/Intuix:user@example.com?secret=...",
  "backupCodes": ["ABCD1234", "EFGH5678", ...]
}
```

### POST /api/0004-auth-mfa/verify-setup

**Request:**
```json
{
  "code": "123456"
}
```

### POST /api/0004-auth-mfa/disable

**Request:**
```json
{
  "password": "string"
}
```

## Database Impact

### Add columns to `auth_users`:

```sql
ALTER TABLE auth_users ADD
    0004-auth-mfa_enabled BIT NOT NULL DEFAULT 0,
    0004-auth-mfa_secret VARBINARY(256) NULL,
    backup_codes NVARCHAR(MAX) NULL;
```

## Risks

1. **Secret storage**: TOTP secrets must be encrypted at rest
2. **Backup code security**: Backup codes must be hashed

## Dependencies

- `IUserRepository`
- `IPasswordHasher`
- `ICurrentUser`

