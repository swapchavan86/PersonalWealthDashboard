# PW-DOC-008 — Implement staging and import lifecycle

## Goal
Stage validated records before canonical financial persistence.

## Acceptance Criteria
- Import lifecycle states are explicit.
- Staged data can be reviewed/rejected before commit.
- Failed imports cannot partially create canonical financial records.
- Import status is auditable.

## Status
Ready
