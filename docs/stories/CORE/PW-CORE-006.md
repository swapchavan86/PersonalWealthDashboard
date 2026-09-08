# PW-CORE-006 — Create application result/error model

## Goal
Standardize typed use-case outcomes and application/domain errors.

## Acceptance Criteria
- Success/failure outcomes are explicit and typed.
- Validation, conflict and not-found semantics are represented without HTTP concerns.
- API mapping can be added later without changing Application semantics.
- Tests cover common outcomes.

## Constraints
Follow SOLID and dependency inversion. Comments minimal and English-only.

## Validation
`dotnet build` and `dotnet test`.

## Status
Ready
