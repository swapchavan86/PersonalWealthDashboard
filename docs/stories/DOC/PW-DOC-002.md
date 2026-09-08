# PW-DOC-002 — Create document metadata and hashing

## Goal
Track source documents and deterministic content identity.

## Acceptance Criteria
- Metadata includes stable document identity, filename, type, size, timestamps, content hash and lifecycle status.
- Hashing is deterministic.
- Duplicate files can be recognized before import.
- No raw secret material is logged.

## Status
Ready
