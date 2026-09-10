# PW-DB-005 — Add tenant isolation/query filtering foundation

## Goal
Prevent cross-tenant data access at the persistence boundary.

## Acceptance Criteria
- Tenant context is represented outside business entities that should not know infrastructure details.
- Tenant-owned entities require tenant identity.
- Reads and writes are tenant-scoped.
- Cross-tenant access tests fail safely.
- No authentication provider is assumed.

## Constraints
Do not rely only on UI/API filtering. Enforce isolation in the application/persistence design.

## Status
Implemented

## Implementation and validation
- Added the Application-owned `ITenantContext` abstraction and Infrastructure `TenantContext` implementation.
- Added EF query filters and `TenantId` indexes for all mapped `ITenantOwned` entities.
- Enforced active-tenant matching for tenant-owned inserts, updates, and deletes in `SaveChanges`/`SaveChangesAsync`.
- Missing or empty tenant context fails safely; no authentication provider or default tenant is used.
- Added focused persistence contract tests for tenant identity, cross-tenant reads/writes, invalid context, and non-tenant entities.
- No migration was required: this story adds enforcement conventions only and the current model contains no mapped tenant-owned business entity requiring schema changes.
- Validation: `dotnet clean`, `dotnet restore`, `dotnet build`, and `dotnet test` passed. Final test totals: 51 passed, 0 failed, 0 skipped.
