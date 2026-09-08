# PW-DOC-010 — Implement canonical import commit and import events

## Goal
Transactionally convert validated staged records into canonical domain data.

## Acceptance Criteria
- Canonical persistence is transactional.
- Events are raised only after successful business persistence according to the event/outbox design.
- Failed commits leave canonical data consistent.
- Import results are auditable.

## Status
Ready
