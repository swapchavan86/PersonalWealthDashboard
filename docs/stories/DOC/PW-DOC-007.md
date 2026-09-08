# PW-DOC-007 — Implement business validation pipeline

## Goal
Validate normalized records against business/domain rules separately from document parsing.

## Acceptance Criteria
- Business validation receives normalized data, not file-library objects.
- Validation is deterministic and testable.
- Validation errors are structured.
- Invalid data cannot reach canonical financial persistence.

## Status
Ready
