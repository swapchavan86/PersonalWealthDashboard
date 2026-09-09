# PW-CORE-007 — Harden architecture test suite

## Goal
Make architecture rules comprehensive and resistant to accidental drift.

## Acceptance Criteria
- Project and namespace dependency rules are covered.
- Forbidden framework/infrastructure dependencies are covered.
- Tests fail on representative violations.
- Test naming and structure are maintainable.

## Constraints
Do not weaken rules for implementation convenience. Comments minimal and English-only.

## Validation
`dotnet restore`, `dotnet build`, and `dotnet test` — pending execution in a local checkout.

## Status
Implemented on `feature/pw-core-007-architecture-hardening`; PR opened for review.
