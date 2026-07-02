# 0009-tenant-isolation - Implementation Plan

## Architecture Design

Query filters are applied in `AuthDbContext.OnModelCreating` using `ICurrentUser.TenantId`. The filter ensures all queries are automatically scoped to the current tenant.

```csharp
modelBuilder.Entity<User>()
    .HasQueryFilter(u => u.TenantId == _currentUser.TenantId);
```

For entities without direct `TenantId`, filtering is achieved through navigation properties or composite filters.

## Domain Changes

No domain changes.

## Application Changes

No application changes.

## Infrastructure Changes

### 1. Add Missing Query Filters

**File:** `Infrastructure/Persistence/AuthDbContext.cs`

Add query filters for:

```csharp
// Company - via Organization
modelBuilder.Entity<Company>()
    .HasQueryFilter(c => c.Organization.TenantId == _currentUser.TenantId);

// UserRole - via User
modelBuilder.Entity<UserRole>()
    .HasQueryFilter(ur => ur.User.TenantId == _currentUser.TenantId);

// UserCompany - via User
modelBuilder.Entity<UserCompany>()
    .HasQueryFilter(uc => uc.User.TenantId == _currentUser.TenantId);

// RefreshToken - via User
modelBuilder.Entity<RefreshToken>()
    .HasQueryFilter(rt => rt.User.TenantId == _currentUser.TenantId);

// RolePermission - via Role
modelBuilder.Entity<RolePermission>()
    .HasQueryFilter(rp => rp.Role.TenantId == _currentUser.TenantId);
```

**Note:** These filters use navigation properties which requires the navigation to be included or the filter to be translatable to SQL. If navigation-based filters are not translatable, alternative approaches include:
- Adding `TenantId` to entities that lack it
- Using raw SQL for filtered queries
- Using `IgnoreQueryFilters()` with explicit tenant checks

### 2. Verify Query Filter Translatability

Test that all query filters are translatable to SQL. If not, add `TenantId` columns to entities that lack them.

## API Changes

No API changes.

## Security Considerations

1. All query filters must be tested
2. `IgnoreQueryFilters()` must be used carefully and logged
3. Raw SQL must include tenant filtering

## Migration Strategy

If adding `TenantId` columns to entities:
1. Add nullable `TenantId` column
2. Backfill from parent entity
3. Make non-nullable
4. Add index

## Testing Strategy

### Architecture Tests

1. `TenantIsolationArchitectureTests`:
   - All tenant-scoped entities have query filters
   - No entity uses `IgnoreQueryFilters()` without justification

### Integration Tests

1. `TenantIsolationTests`:
   - Create data in Tenant A
   - Query as Tenant B → empty results
   - Verify cross-tenant access is impossible

### Unit Tests

1. `QueryFilterTests`:
   - Each entity's filter works correctly
   - Filter uses `ICurrentUser.TenantId`

## Rollback Strategy

1. Remove query filters from `AuthDbContext`
2. No database changes (unless columns were added)

