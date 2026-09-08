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
`dotnet restore`, `dotnet build`, and `dotnet test` — pending execution in a local checkout.

## Status
Implemented on `feature/pw-core-005-domain-event-contract`; PR opened for review.
