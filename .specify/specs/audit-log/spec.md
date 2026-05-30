# audit-log - Specification

## Business Context

Security and compliance require tracking who did what and when. Audit logging records significant events like logins, logouts, permission changes, and data modifications. This provides an immutable trail for security investigations and compliance audits.

## Problem Statement

Currently, no audit logging exists beyond basic logging via `ILogger`. A structured audit log system is needed to track security-relevant events with user, tenant, timestamp, and action details.

## Goals

1. Record security-relevant events in an audit log table
2. Track user actions (login, logout, CRUD operations)
3. Store event metadata (user, tenant, IP, timestamp, action)
4. Provide API to query audit logs
5. Make audit logs immutable (append-only)

## Non-Goals

- Real-time alerting
- Log aggregation (use Serilog/ELK)
- Compliance reporting
- Data change tracking (before/after values)

## Functional Requirements

1. **FR-01**: System must record audit events for security-relevant actions
2. **FR-02**: Each event must include: userId, tenantId, action, entity, entityId, timestamp, ipAddress, userAgent
3. **FR-03**: Audit logs must be append-only (no updates or deletes)
4. **FR-04**: Admins can query audit logs with filters (user, action, date range)
5. **FR-05**: Audit logs are tenant-scoped
6. **FR-06**: Require `AUDIT_VIEW` permission to query logs

## Non-Functional Requirements

1. **NFR-01**: Audit logging must not block the main request
2. **NFR-02**: Audit log writes must be fire-and-forget (async)
3. **NFR-03**: Audit logs must be retained for 90 days minimum

## Acceptance Criteria

- [ ] Login events are recorded
- [ ] Logout events are recorded
- [ ] Failed login attempts are recorded
- [ ] Permission changes are recorded
- [ ] User CRUD operations are recorded
- [ ] GET /api/audit-logs returns filtered list
- [ ] Audit logs are tenant-scoped
- [ ] Audit logs cannot be modified or deleted

## Events to Track

| Event | Action Code |
|-------|------------|
| Login Success | `AUTH_LOGIN_SUCCESS` |
| Login Failed | `AUTH_LOGIN_FAILED` |
| Logout | `AUTH_LOGOUT` |
| Token Refresh | `AUTH_TOKEN_REFRESH` |
| Password Reset | `USER_PASSWORD_RESET` |
| Account Locked | `USER_ACCOUNT_LOCKED` |
| Role Assigned | `ROLE_ASSIGNED` |
| Role Removed | `ROLE_REMOVED` |
| Permission Changed | `PERMISSION_CHANGED` |
| User Created | `USER_CREATED` |
| User Updated | `USER_UPDATED` |

## API Contract

### GET /api/audit-logs

**Query Parameters:**
- `userId` (optional)
- `action` (optional)
- `fromDate` (optional)
- `toDate` (optional)
- `page` (default 1)
- `pageSize` (default 20, max 100)

**Response (200):**
```json
{
  "items": [
    {
      "id": "guid",
      "userId": "guid",
      "username": "string",
      "action": "AUTH_LOGIN_SUCCESS",
      "entity": "User",
      "entityId": "guid",
      "details": "string",
      "ipAddress": "192.168.1.1",
      "userAgent": "Mozilla/5.0...",
      "timestamp": "2026-05-29T10:00:00Z"
    }
  ],
  "total": 100,
  "page": 1,
  "pageSize": 20
}
```

## Database Impact

### New Table: `auth_audit_logs`

```sql
CREATE TABLE auth_audit_logs (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    tenant_id UNIQUEIDENTIFIER NOT NULL,
    user_id UNIQUEIDENTIFIER NULL,
    username NVARCHAR(100) NULL,
    action NVARCHAR(50) NOT NULL,
    entity NVARCHAR(50) NULL,
    entity_id UNIQUEIDENTIFIER NULL,
    details NVARCHAR(500) NULL,
    ip_address VARCHAR(45) NULL,
    user_agent NVARCHAR(300) NULL,
    timestamp DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    FOREIGN KEY (tenant_id) REFERENCES auth_tenants(id)
);
```

## Risks

1. **Performance**: Audit logging adds overhead. Mitigated by async/fire-and-forget.
2. **Storage**: High-traffic systems generate many logs. Mitigated by retention policy.

## Dependencies

- `ICurrentUser` - User context
- `IHttpContextAccessor` - IP and User-Agent
- `AuthDbContext` - Database access
