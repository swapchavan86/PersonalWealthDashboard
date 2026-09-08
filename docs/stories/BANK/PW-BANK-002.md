# PW-BANK-002 — Create bank transaction domain model

## Goal
Create canonical bank transactions independent of source files.

## Acceptance Criteria
- Transaction date, amount, direction, description and account ownership are modeled.
- Source/import identity can be retained for audit/idempotency.
- Domain invariants prevent invalid transaction states.
- Tenant ownership is enforced.

## Status
Ready
