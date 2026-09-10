# PW-EXP-002 — Implement expense application use cases

## Goal
Implement create, update and query expense workflows through Application interfaces.

## Acceptance Criteria
- Application contracts expose category and expense workflows without infrastructure leakage.
- Create, update, get and list operations validate tenant ownership and business invariants.
- Application orchestration depends on abstractions rather than EF Core or provider implementations.
- Success, validation, not-found and conflict outcomes use the application result model.

## Status
Implemented
