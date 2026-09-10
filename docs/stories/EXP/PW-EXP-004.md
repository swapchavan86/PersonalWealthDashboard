# PW-EXP-004 — Implement expense aggregation/reporting queries

## Goal
Provide application query models for category, month and trend analysis without leaking database concerns.

## Acceptance Criteria
- Reporting is tenant-scoped.
- Category totals and monthly totals are available through Application-facing models.
- Trend points are deterministic for a requested date range.
- Reporting abstractions do not expose EF Core entities or database implementation details.

## Status
Implemented
