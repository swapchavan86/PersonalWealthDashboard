# PW-DOC-006 — Implement schema and structural validation

## Goal
Reject structurally invalid documents before business processing.

## Acceptance Criteria
- Required columns/fields are checked.
- Dates, numbers and formats are validated.
- Unsupported template versions are rejected.
- Validation errors identify row/field where possible.
- Parser and business rules remain separate.

## Status
Implemented
