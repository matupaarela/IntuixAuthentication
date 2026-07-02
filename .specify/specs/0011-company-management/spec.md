# 0011-company-management - Specification

## Business Context

Companies exist within organizations and represent legal entities. Users are assigned to companies and switch between them. Company management enables CRUD operations with organization scoping.

## Problem Statement

No API exists for managing companies. The `Company` entity exists but is only used for company switching. An administrative API is needed.

## Goals

1. CRUD operations for companies
2. Organization-scoped isolation
3. Validate company belongs to organization
4. Manage user-company assignments

## Non-Goals

- Company hierarchy beyond organization
- Company-specific configuration

## Functional Requirements

1. **FR-01**: List companies within an organization
2. **FR-02**: Get company by ID
3. **FR-03**: Create company within an organization
4. **FR-04**: Update company details (name, RUC, active status)
5. **FR-05**: Assign user to company
6. **FR-06**: Remove user from company
7. **FR-07**: Set default company for user
8. **FR-08**: Require `COMPANY_MANAGE` permission

## Acceptance Criteria

- [ ] GET /api/companies?organizationId= returns list
- [ ] POST /api/companies creates company
- [ ] PUT /api/companies/{id} updates company
- [ ] POST /api/companies/{id}/users assigns user
- [ ] DELETE /api/companies/{id}/users/{userId} removes user
- [ ] PUT /api/companies/{id}/users/{userId}/default sets default
- [ ] Cross-organization access returns 404

## API Contract

### GET /api/companies

**Query:** `organizationId` (required)

### POST /api/companies

**Request:**
```json
{
  "organizationId": "guid",
  "name": "string",
  "ruc": "string (optional)"
}
```

### POST /api/companies/{id}/users

**Request:**
```json
{
  "userId": "guid",
  "isDefault": false
}
```

## Database Impact

- Reads/writes to: `auth_companies`, `auth_user_companies`

## Dependencies

- `ICompanyRepository`
- `IUserRepository`
- `ICurrentUser`

