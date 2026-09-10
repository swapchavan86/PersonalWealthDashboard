# PW-DB-004 — Add audit and concurrency conventions

## Goal
Standardize audit metadata and optimistic concurrency where appropriate.

## Acceptance Criteria
- Created/Updated metadata is consistently modeled.
- Concurrency strategy is explicit for mutable business records where needed.
- Audit behavior is test-covered.
- No sensitive values are written to logs/audit fields accidentally.

## Status
Implemented on feature/pw-db-004-audit-concurrency; local validation pending.
