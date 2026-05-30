# api-keys - Implementation Plan

## Architecture Design

```
ApiKeysController
  → POST /api/api-keys → ApiKeyGenerateCommand
  → GET /api/api-keys → ApiKeyGetListQuery
  → DELETE /api/api-keys/{id} → ApiKeyRevokeCommand
```

## Domain Changes

### 1. Create ApiKey Entity

**File:** `Domain/Entities/ApiKey.cs`

```csharp
public class ApiKey
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public byte[] KeyHash { get; set; } = default!;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## Application Changes

### 1. Create Feature Folder

```
Application/ApiKeys/
├── Commands/
│   ├── ApiKeyGenerateCommand.cs/.Handler.cs
│   └── ApiKeyRevokeCommand.cs/.Handler.cs
├── Queries/
│   └── ApiKeyGetListQuery.cs/.Handler.cs
├── DTOs/
│   ├── ApiKeyGenerateRequest.cs
│   ├── ApiKeyGenerateResponse.cs
│   └── ApiKeyResponse.cs
├── Validators/
└── Interfaces/
    └── IApiKeyRepository.cs
```

## Infrastructure Changes

### 1. Create ApiKeyConfiguration

### 2. Add DbSet to AuthDbContext

### 3. Create ApiKeyRepository

### 4. Add migration

## API Changes

### 1. Create ApiKeysController

CRUD endpoints with `APIKEY_MANAGE` permission.

## Security Considerations

1. Raw key returned once
2. Key stored hashed
3. Tenant-scoped

## Migration Strategy

1. Create migration for auth_api_keys table

## Testing Strategy

1. Unit tests for handlers
2. Integration tests for endpoints

## Rollback Strategy

1. Remove new files
2. Drop migration
