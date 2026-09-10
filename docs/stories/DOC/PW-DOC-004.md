# PW-DOC-004 — Define import template abstraction

## Goal
Separate document parsing from financial business logic.

## Acceptance Criteria
- Application-facing template/parser contracts are defined in the correct layer.
- Concrete PDF/Excel/CSV libraries remain outside Domain/Application business rules.
- Template selection is explicit and testable.

## Status
Implemented
