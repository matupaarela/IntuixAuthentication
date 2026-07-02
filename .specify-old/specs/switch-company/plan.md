# switch-company - Implementation Plan

## Architecture Design

```
AuthController.SwitchCompany()
  → IMediator.Send(SwitchCompanyCommand)
    → SwitchCompanyCommandHandler.Handle()
      → ICompanyRepository.GetByIdAsync(companyId)
      → Validate: exists, active, same tenant
      → IUserRepository.GetUserCompaniesAsync(userId)
      → Validate: user assigned to company
      → IJwtProvider.GenerateToken(user, companyId, roles, permissions)
    ← AuthResponse
  ← 200 OK
```

## Domain Changes

No domain changes required.

## Application Changes

### 1. Create ICompanyRepository

**File:** `Application/Auth/Interfaces/ICompanyRepository.cs`

```csharp
public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id);
}
```

### 2. Enhance SwitchCompanyCommandHandler

**File:** `Application/Auth/Commands/SwitchCompany/SwitchCompanyCommandHandler.cs`

Changes:
- Inject `ICompanyRepository`
- Validate company exists
- Validate company is active
- Validate company belongs to user's tenant
- Load full user entity for JWT generation

## Infrastructure Changes

### 1. Create CompanyRepository

**File:** `Infrastructure/Persistence/Repositories/CompanyRepository.cs`

Implement `ICompanyRepository.GetByIdAsync()`.

### 2. Register ICompanyRepository

**File:** `Api/Program.cs`

Add DI registration.

## API Changes

No API changes required. Controller already dispatches `SwitchCompanyCommand`.

## Security Considerations

1. Company must belong to user's tenant
2. User must be assigned to the company
3. Company must be active

## Migration Strategy

No migration required.

## Testing Strategy

### Unit Tests

1. `SwitchCompanyCommandHandlerTests`:
   - Valid company → new JWT
   - Non-existent company → throws
   - Inactive company → throws
   - Unauthorized company → throws
   - Cross-tenant company → throws

### Integration Tests

1. `SwitchCompanyEndpointTests`:
   - POST /auth/switch-company → 200
   - Invalid company → 400
   - Unauthenticated → 401

## Rollback Strategy

1. Revert handler changes
2. Remove CompanyRepository
3. No database changes
