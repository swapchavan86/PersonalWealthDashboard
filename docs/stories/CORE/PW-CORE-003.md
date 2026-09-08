# PW-CORE-003 — Create base entity contracts

## Goal
Create framework-independent base entity/value contracts for identity and common metadata.

## Acceptance Criteria
- Domain-owned contracts define stable identity semantics.
- No EF Core, ASP.NET Core or Infrastructure references.
- Equality/identity semantics are test-covered.
- Contracts are reusable by future modules.

## Constraints
Apply SOLID and avoid speculative abstractions. Comments must be minimal and English-only.

## Validation
`dotnet build` and `dotnet test`.

## Status
Ready
