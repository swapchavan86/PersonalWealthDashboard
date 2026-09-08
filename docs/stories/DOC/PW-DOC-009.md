# PW-DOC-009 — Implement duplicate detection and idempotent import

## Goal
Make repeated document and row imports safe.

## Acceptance Criteria
- File content identity is used for document-level duplicate detection.
- Row-level stable fingerprints are used where source identity is insufficient.
- Re-importing the same source does not create duplicate canonical records.
- Duplicate decisions are auditable.

## Status
Ready
