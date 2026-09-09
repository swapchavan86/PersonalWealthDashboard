# PW-DB-002 — Configure EF Core Code First conventions

## Goal
Establish the persistence foundation using EF Core Code First.

## Acceptance Criteria
- DbContext lives in Infrastructure.
- Domain remains EF-independent.
- Central conventions define keys, precision, timestamps and mapping behavior.
- Entity configurations are testable.
- No database-first scaffolding.

## Constraints
Schema follows domain/business requirements. Avoid UI-driven schema design.

## Validation
Build, unit tests and integration mapping tests.

## Status
Implemented on `feature/pw-db-002-ef-core-conventions`; local validation pending.
