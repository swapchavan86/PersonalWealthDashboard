# PW-BANK-008 — Add banking integration tests and invariants

## Goal
Validate the complete first-bank document-to-canonical-data flow.

## Acceptance Criteria
- Tests cover valid statement import.
- Tests cover malformed rows and validation failures.
- Tests cover duplicate/re-import behavior.
- Tests cover reconciliation mismatch.
- Tests cover transfer detection.
- Tests cover tenant isolation.
- Tests prove no partial canonical persistence on failure.

## Status
Ready
