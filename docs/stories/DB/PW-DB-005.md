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
Ready
