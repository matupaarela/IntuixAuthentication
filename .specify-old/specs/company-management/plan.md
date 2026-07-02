# company-management - Implementation Plan

## Architecture Design

```
CompaniesController
  → GET /api/companies → CompanyGetListQuery
  → GET /api/companies/{id} → CompanyGetByIdQuery
  → POST /api/companies → CompanyCreateCommand
  → PUT /api/companies/{id} → CompanyUpdateCommand
  → POST /api/companies/{id}/users → CompanyAssignUserCommand
  → DELETE /api/companies/{id}/users/{userId} → CompanyRemoveUserCommand
  → PUT /api/companies/{id}/users/{userId}/default → CompanySetDefaultUserCommand
```

## Domain Changes

No domain changes.

## Application Changes

### 1. Create Feature Folder

```
Application/Companies/
├── Commands/
│   ├── CompanyCreateCommand.cs/.Handler.cs
│   ├── CompanyUpdateCommand.cs/.Handler.cs
│   ├── CompanyAssignUserCommand.cs/.Handler.cs
│   ├── CompanyRemoveUserCommand.cs/.Handler.cs
│   └── CompanySetDefaultUserCommand.cs/.Handler.cs
├── Queries/
│   ├── CompanyGetListQuery.cs/.Handler.cs
│   └── CompanyGetByIdQuery.cs/.Handler.cs
├── DTOs/
│   ├── CompanyResponse.cs
│   ├── CompanyCreateRequest.cs
│   └── CompanyUpdateRequest.cs
├── Validators/
└── Interfaces/
    └── ICompanyRepository.cs
```

### 2. Create ICompanyRepository

Extend with:
- `GetByOrganizationAsync(Guid orgId, int page, int pageSize)`
- `GetCountByOrganizationAsync(Guid orgId)`
- `GetByIdAsync(Guid id)`
- `AddAsync(Company company)`
- `UpdateAsync(Company company)`
- `AssignUserAsync(Guid companyId, Guid userId, bool isDefault)`
- `RemoveUserAsync(Guid companyId, Guid userId)`
- `SetDefaultUserAsync(Guid companyId, Guid userId)`

## Infrastructure Changes

### 1. Implement CompanyRepository

All queries scoped to organization.

## API Changes

### 1. Create CompaniesController

CRUD + user assignment endpoints with `COMPANY_MANAGE` permission.

## Security Considerations

1. Organization scoping enforced
2. Permission checks on all endpoints

## Migration Strategy

No migration required.

## Testing Strategy

1. Unit tests for all handlers
2. Integration tests for endpoints

## Rollback Strategy

1. Remove new files
2. No database changes
