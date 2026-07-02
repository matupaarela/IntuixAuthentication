# device-management - Implementation Plan

## Architecture Design

```
DevicesController
  → GET /api/devices → DeviceGetListQuery
  → DELETE /api/devices/{tokenId} → DeviceRevokeSessionCommand
  → POST /api/devices/revoke-all → DeviceRevokeAllSessionsCommand
```

## Domain Changes

No domain changes.

## Application Changes

### 1. Create Feature Folder

```
Application/Devices/
├── Commands/
│   ├── DeviceRevokeSessionCommand.cs/.Handler.cs
│   └── DeviceRevokeAllSessionsCommand.cs/.Handler.cs
├── Queries/
│   └── DeviceGetListQuery.cs/.Handler.cs
├── DTOs/
│   └── DeviceSessionResponse.cs
└── Interfaces/
    └── IRefreshTokenRepository.cs (extend existing)
```

### 2. Extend IRefreshTokenRepository

Add methods:
- `Task<List<RefreshToken>> GetActiveSessionsByUserAsync(Guid userId)`
- `Task RevokeSessionAsync(Guid tokenId, Guid userId)`
- `Task RevokeAllSessionsExceptCurrentAsync(Guid userId, Guid currentTokenId)`

## Infrastructure Changes

### 1. Extend RefreshTokenRepository

Implement new methods.

## API Changes

### 1. Create DevicesController

Endpoints with `[Authorize]`.

## Security Considerations

1. Users can only see/revoke their own sessions
2. Current session cannot be revoked via revoke-all

## Migration Strategy

No migration required.

## Testing Strategy

1. Unit tests for handlers
2. Integration tests for endpoints

## Rollback Strategy

1. Remove new files
2. No database changes
