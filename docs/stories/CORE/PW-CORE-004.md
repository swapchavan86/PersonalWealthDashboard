# PW-CORE-004 — Create tenant-aware entity contract

## Goal
Make tenant ownership a first-class concern from the first persisted business entity.

## Acceptance Criteria
- Tenant-aware contract exposes TenantId without infrastructure coupling.
- Tenant identity cannot be silently omitted from tenant-owned entities.
- Tests cover tenant ownership semantics.
- No authentication provider is introduced yet.

## Constraints
Follow dependency inversion and strong SOLID boundaries. Comments minimal, English-only.

## Validation
`dotnet build` and `dotnet test`.

## Status
Ready
