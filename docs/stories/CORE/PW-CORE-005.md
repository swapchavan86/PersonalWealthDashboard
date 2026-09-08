# PW-CORE-005 — Create domain event contract

## Goal
Define immutable framework-independent domain event contracts.

## Acceptance Criteria
- Event identity and occurrence metadata are represented.
- Contract has no event-bus implementation dependency.
- Domain events are testable and suitable for outbox publication later.

## Constraints
No infrastructure or business feature implementation. Comments minimal and English-only.

## Validation
`dotnet build` and `dotnet test`.

## Status
Ready
