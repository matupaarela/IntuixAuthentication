# Tasks: Multi-Tenant Auth Hardening

**Input**: Design documents from `.specify/specs/0018-multi-tenant-auth-hardening/`
**Prerequisites**: `plan.md` (required), `spec.md` (required), `research.md`, `data-model.md`, `contracts/`

**Organization**: Tasks are grouped by user story so each story can be implemented and validated independently.

**Test Policy**: Tests are included because the feature is security-critical and the spec defines explicit independent test criteria per user story.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project setup and baseline infrastructure for the auth hardening work.

- [ ] T001 [P] Create EF migration scaffolding in `Intuix.Authentication.Infrastructure/Migrations/` and align the SQL baseline in `Intuix.Authentication.Infrastructure/Scripts/Intuix.Authentication.sql`
- [ ] T002 Add centralized ProblemDetails middleware in `Intuix.Authentication.Api/Middleware/ExceptionHandlingMiddleware.cs` and register it in `Intuix.Authentication.Api/Program.cs`
- [ ] T003 Move the tenant guard before authorization in `Intuix.Authentication.Api/Program.cs` and harden `Intuix.Authentication.Api/Middleware/TenantMiddleware.cs` to fail closed without raw exceptions

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared auth infrastructure that must be complete before any user story can be delivered.

**Checkpoint**: No user story work should start until this phase is complete.

- [ ] T004 Add `CancellationToken` support to auth repository interfaces and implementations in `Intuix.Authentication.Application/Auth/Interfaces/IUserRepository.cs`, `ITenantRepository.cs`, `IRefreshTokenRepository.cs`, and `Intuix.Authentication.Infrastructure/Persistence/Repositories/UserRepository.cs`, `TenantRepository.cs`, `RefreshTokenRepository.cs`
- [ ] T005 Add `SaveChangesAsync(CancellationToken)` support for login state persistence in `Intuix.Authentication.Application/Auth/Interfaces/IUserRepository.cs` and `Intuix.Authentication.Infrastructure/Persistence/Repositories/UserRepository.cs`
- [ ] T006 [P] Backfill `LastUsedAt` for existing active refresh-token rows and keep `Intuix.Authentication.Infrastructure/Migrations/*` and `Intuix.Authentication.Infrastructure/Scripts/Intuix.Authentication.sql` aligned
- [ ] T007 [P] Replace `ToLower()` lookups with index-friendly case-insensitive tenant/user comparisons in `Intuix.Authentication.Infrastructure/Persistence/Repositories/TenantRepository.cs` and `Intuix.Authentication.Infrastructure/Persistence/Repositories/UserRepository.cs`

**Checkpoint**: Foundation ready - user story implementation can now begin.

---

## Phase 3: User Story 1 - Secure Sign-In and Lockout (Priority: P1) 🎯 MVP

**Goal**: A tenant user can sign in securely, failed attempts persist durably, and lockout remains manual.

**Independent Test**: Valid credentials return a session; repeated failures increment and persist the lockout counter; wrong-tenant or wrong-password attempts do not reveal whether the account exists.

### Tests for User Story 1

- [ ] T008 [P] [US1] Add sign-in and lockout regression tests in `tests/Intuix.Authentication.ArchitectureTests/LoginHardeningTests.cs`, including the 5-attempt threshold
- [ ] T009 [P] [US1] Add login contract and generic error assertions in `tests/Intuix.Authentication.ArchitectureTests/AuthLoginContractTests.cs`

### Implementation for User Story 1

- [ ] T010 [US1] Wire login persistence through the updated repository contract in `Intuix.Authentication.Application/Auth/Interfaces/IUserRepository.cs` and `Intuix.Authentication.Infrastructure/Persistence/Repositories/UserRepository.cs`
- [ ] T011 [US1] Persist failed-attempt counts, lockout status, and last-login updates in `Intuix.Authentication.Application/Auth/Commands/Login/LoginCommandHandler.cs`
- [ ] T012 [US1] Keep login request/response handling aligned with the spec in `Intuix.Authentication.Api/Controllers/AuthController.cs` and `Intuix.Authentication.Application/Auth/DTOs/AuthResponse.cs`

**Checkpoint**: User Story 1 should now be fully functional and independently testable.

---

## Phase 4: User Story 2 - Session Renewal and Reuse Detection (Priority: P1)

**Goal**: Refresh-token renewal rotates the session, and revoked-token reuse invalidates the related family.

**Independent Test**: A refresh token can be used once successfully; reusing the revoked token revokes the session family and yields a generic session error.

### Tests for User Story 2

- [ ] T013 [P] [US2] Add refresh rotation and reuse-detection tests in `tests/Intuix.Authentication.ArchitectureTests/RefreshTokenHardeningTests.cs`
- [ ] T014 [P] [US2] Add token-family revocation regression tests in `tests/Intuix.Authentication.ArchitectureTests/RefreshTokenReuseTests.cs`

### Implementation for User Story 2

- [ ] T015 [US2] Update `Intuix.Authentication.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs` and `Intuix.Authentication.Application/Auth/Interfaces/IRefreshTokenRepository.cs` for cancellation-aware chain revocation and session lookup
- [ ] T016 [US2] Update `Intuix.Authentication.Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs` to rotate refresh tokens, detect reuse, revoke the family, and stamp `LastUsedAt`

**Checkpoint**: User Story 2 should now be fully functional and independently testable.

---

## Phase 5: User Story 3 - Tenant and Permission Enforcement (Priority: P1)

**Goal**: Protected endpoints enforce tenant boundaries and permission policies consistently.

**Independent Test**: Missing-permission requests are denied; cross-tenant requests are rejected; requests without trusted tenant context fail closed.

### Tests for User Story 3

- [ ] T017 [P] [US3] Add tenant-isolation and permission-policy regression tests in `tests/Intuix.Authentication.ArchitectureTests/AuthorizationHardeningTests.cs`

### Implementation for User Story 3

- [ ] T018 [US3] Define the endpoint permission matrix in `Intuix.Authentication.Api/Authorization/EndpointPermissions.cs` and apply the policy names consistently across auth/device controllers
- [ ] T019 [US3] Apply explicit permission policies to protected auth/device endpoints in `Intuix.Authentication.Api/Controllers/AuthController.cs` and `Intuix.Authentication.Api/Controllers/DevicesController.cs`

**Checkpoint**: User Story 3 should now be fully functional and independently testable.

---

## Phase 6: User Story 4 - Company Switching and Session Controls (Priority: P2)

**Goal**: A user can switch company context and manage active sessions without crossing tenant boundaries.

**Independent Test**: A user assigned to multiple companies can switch context, list active sessions, revoke one session, and revoke all others while preserving the current session.

### Tests for User Story 4

- [ ] T020 [P] [US4] Add company-switch and session-control regression tests in `tests/Intuix.Authentication.ArchitectureTests/CompanySwitchAndDevicesTests.cs`
- [ ] T021 [P] [US4] Add device-session history and current-session marker tests in `tests/Intuix.Authentication.ArchitectureTests/DeviceSessionHistoryTests.cs`

### Implementation for User Story 4

- [ ] T022 [US4] Tighten user-company lookup rules for default-company and tenant ownership in `Intuix.Authentication.Infrastructure/Persistence/Repositories/UserRepository.cs`
- [ ] T023 [US4] Enforce same-tenant, active-membership company switching in `Intuix.Authentication.Application/Auth/Commands/SwitchCompany/SwitchCompanyCommandHandler.cs`
- [ ] T024 [US4] Populate current-session markers and `LastUsedAt` in `Intuix.Authentication.Application/Devices/Queries/DeviceGetListQueryHandler.cs`
- [ ] T025 [US4] Keep session revocation tenant-safe in `Intuix.Authentication.Application/Auth/Commands/Logout/LogoutCommandHandler.cs`, `Intuix.Authentication.Application/Auth/Commands/Logout/LogoutAllCommandHandler.cs`, `Intuix.Authentication.Application/Devices/Commands/DeviceRevokeSessionCommandHandler.cs`, `Intuix.Authentication.Application/Devices/Commands/DeviceRevokeAllSessionsCommandHandler.cs`, and `Intuix.Authentication.Infrastructure/Persistence/Repositories/RefreshTokenRepository.cs`

**Checkpoint**: All user stories should now be independently functional.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final hardening and documentation updates that affect multiple stories.

- [ ] T026 [P] Update Swagger descriptions and security notes in `Intuix.Authentication.Api/Swagger/AuthorizeOperationFilter.cs`, `Intuix.Authentication.Api/Controllers/AuthController.cs`, and `Intuix.Authentication.Api/Controllers/DevicesController.cs`
- [X] T027 [P] Update `.specify/specs/0018-multi-tenant-auth-hardening/quickstart.md` with final validation notes and auth flow steps
- [X] T028 [P] Run `dotnet test IntuixAuthentication.sln` and fix remaining regressions in `tests/Intuix.Authentication.ArchitectureTests/*`
- [ ] T029 [P] Verify the SQL baseline and migration artifacts stay aligned in `Intuix.Authentication.Infrastructure/Scripts/Intuix.Authentication.sql` and `Intuix.Authentication.Infrastructure/Migrations/*`
- [X] T030 [P] Add auth and session validators in `Intuix.Authentication.Application/Auth/Validators/` and `Intuix.Authentication.Application/Devices/Validators/`
- [X] T031 [P] Add structured security logging with tenant/session correlation in `Intuix.Authentication.Api/Program.cs`, `Intuix.Authentication.Api/Middleware/ExceptionHandlingMiddleware.cs`, `Intuix.Authentication.Api/Middleware/TenantMiddleware.cs`, and auth/device handlers
- [ ] T032 [P] Align the shared API error contract with generic security failures in `.specify/specs/0018-multi-tenant-auth-hardening/contracts/api.md`
- [ ] T033 [P] Add performance/load validation for sign-in, refresh, company-switch, and session-control flows in `tests/Intuix.Authentication.ArchitectureTests/PerformanceHardeningTests.cs`
- [X] T034 [P] Document the support/operations manual-unlock audit event schema and ownership in `.specify/specs/0018-multi-tenant-auth-hardening/quickstart.md` and `.specify/specs/0018-multi-tenant-auth-hardening/research.md`
- [X] T035 [P] Validate rollback behavior by reverting `Intuix.Authentication.Api` and `Intuix.Authentication.Application` against the additive schema and document the recovery path in `.specify/specs/0018-multi-tenant-auth-hardening/quickstart.md`
- [X] T036 [P] Add an HTTP integration test for `POST /auth/login` in `tests/Intuix.Authentication.ArchitectureTests/AuthEndpointIntegrationTests.cs` using the seeded `TNT-INTUIX` / `admin` / `Admin123!` credentials

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion and blocks all user stories.
- **User Stories (Phase 3+)**: All depend on Foundational completion.
- **Polish (Final Phase)**: Depends on all desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational; no dependency on other stories.
- **User Story 2 (P1)**: Can start after Foundational; no dependency on other stories.
- **User Story 3 (P1)**: Can start after Foundational; permission matrix + policy application are self-contained.
- **User Story 4 (P2)**: Can start after Foundational; may reuse shared session primitives but remains independently testable.

### Within Each User Story

- Tests first, implementation second.
- Repository/interface changes before handlers that depend on them.
- Story complete before moving to the next priority.

### Parallel Opportunities

- Setup tasks marked `[P]` can run in parallel when they touch different files.
- Foundational tasks `T006` and `T007` can run in parallel.
- User Story 1 tests `T008` and `T009` can run in parallel.
- User Story 2 tests `T013` and `T014` can run in parallel.
- User Story 3 test `T017` can run in parallel with implementation prep.
- User Story 4 tests `T020` and `T021` can run in parallel.
- Polish tasks marked `[P]` can run in parallel.

---

## Parallel Example: User Story 1

```text
Task: T008 [P] [US1] Add sign-in and lockout regression tests in `tests/Intuix.Authentication.ArchitectureTests/LoginHardeningTests.cs`
Task: T009 [P] [US1] Add login contract and generic error assertions in `tests/Intuix.Authentication.ArchitectureTests/AuthLoginContractTests.cs`
```

## Parallel Example: User Story 2

```text
Task: T013 [P] [US2] Add refresh rotation and reuse-detection tests in `tests/Intuix.Authentication.ArchitectureTests/RefreshTokenHardeningTests.cs`
Task: T014 [P] [US2] Add token-family revocation regression tests in `tests/Intuix.Authentication.ArchitectureTests/RefreshTokenReuseTests.cs`
```

## Parallel Example: User Story 4

```text
Task: T020 [P] [US4] Add company-switch and session-control regression tests in `tests/Intuix.Authentication.ArchitectureTests/CompanySwitchAndDevicesTests.cs`
Task: T021 [P] [US4] Add device-session history and current-session marker tests in `tests/Intuix.Authentication.ArchitectureTests/DeviceSessionHistoryTests.cs`
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. STOP and validate User Story 1 independently.

### Incremental Delivery

1. Setup + Foundational -> shared auth hardening foundation.
2. Add User Story 1 -> MVP sign-in hardening.
3. Add User Story 2 -> refresh rotation and reuse protection.
4. Add User Story 3 -> tenant and permission enforcement.
5. Add User Story 4 -> company switching and session controls.
6. Finish with Polish tasks and final regression pass.
