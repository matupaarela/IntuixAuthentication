# 0001-auth-login - Tasks

## Implementation Tasks

### Task 1: Fix Password Verification in LoginCommandHandler
- **Priority:** High
- **Status:** Completed
- **File:** `Application/Auth/Commands/Login/LoginCommandHandler.cs`
- **Changes:**
  - Replace `if (request.Password != user.PasswordHash)` with `if (!_hasher.Verify(request.Password, user.PasswordHash))`
  - The `_hasher` field is already injected
- **Validation:** Password is verified using PBKDF2 hash comparison

### Task 2: Add Structured Logging
- **Priority:** Medium
- **Status:** Pending
- **File:** `Application/Auth/Commands/Login/LoginCommandHandler.cs`
- **Changes:**
  - Add `ILogger<LoginCommandHandler>` injection
  - Log successful login with user ID
  - Log failed login with username (no password)
  - Log account lockout events
- **Validation:** Login events appear in structured logs

### Task 3: Write Unit Tests for LoginCommandHandler
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.UnitTests/Auth/Commands/LoginCommandHandlerTests.cs`
- **Test Cases:**
  - Valid credentials → returns AuthResponse
  - Invalid username → throws Exception("Invalid credentials")
  - Invalid password → throws Exception("Invalid credentials"), FailedAttempts incremented
  - 5th failed attempt → account locked
  - Locked account → throws Exception("User locked")
  - Successful login → FailedAttempts reset, LastLogin updated
  - Invalid tenant → throws Exception("Invalid tenant")
  - No company assigned → throws Exception("User has no company assigned")
- **Validation:** All unit tests pass

### Task 4: Write Integration Tests for Login Endpoint
- **Priority:** High
- **Status:** Pending
- **File:** `tests/Intuix.Authentication.IntegrationTests/Api/LoginEndpointTests.cs`
- **Test Cases:**
  - POST /auth/login with valid credentials → 200 with tokens
  - POST /auth/login with invalid credentials → 400
  - POST /auth/login with locked account → 400
  - POST /auth/login with missing fields → 400
- **Validation:** All integration tests pass

### Task 5: Verify JWT Claims
- **Priority:** Medium
- **Status:** Completed
- **Validation:**
  - Decode JWT from login response
  - Verify claims: sub, tenant, company, role, perm, jti
  - Verify expiry is 15 minutes from now
  - Verify issuer and audience match configuration
- **Method:** Unit test that generates token and decodes it

### Task 6: Verify Refresh Token Storage
- **Priority:** Medium
- **Status:** Completed
- **Validation:**
  - After login, refresh token hash exists in auth_refresh_tokens
  - Token hash matches SHA256 of returned refresh token
  - ExpiresAt is 7 days from creation
  - UserId matches logged-in user
- **Method:** Integration test that queries database after login

## Documentation Tasks

### Task 7: Document Login Flow
- **Priority:** Medium
- **Status:** Pending
- **File:** `docs/0001-auth-login.md`
- **Changes:** Document the login flow, tenant resolution, password hashing, and error handling
- **Validation:** Documentation is complete

## Validation Checkpoints

- [X] Password verification uses IPasswordHasher
- [X] No plain text password comparison
- [X] Generic error messages returned
- [X] JWT contains all required claims
- [X] Refresh token is stored hashed
- [ ] Account lockout works after 5 attempts
- [X] FailedAttempts resets on success
- [X] LastLogin is updated on success
- [ ] All unit tests pass
- [ ] All integration tests pass
- [X] No sensitive data in logs

