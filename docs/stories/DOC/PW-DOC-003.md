# PW-DOC-003 — Implement folder scanner and file discovery

## Goal
Discover supported documents without mixing discovery with business import.

## Acceptance Criteria
- Scanner is deterministic and configuration-driven.
- Unsupported, temporary and unsafe files are ignored/rejected.
- Discovery produces document candidates, not canonical financial records.
- Tests cover repeated scans.

## Status
Implemented

## Implementation
- Added `IDocumentScanner` and `FileSystemDocumentScanner`.
- Discovery is top-level, extension-filtered, deterministic and excludes temporary Office lock files.
- Scanner returns document candidates only and performs no import or canonical persistence.
