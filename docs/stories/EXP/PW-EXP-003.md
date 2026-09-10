# PW-EXP-003 — Implement recurring expense rules

## Goal
Model recurring expense definitions and occurrence logic deterministically.

## Acceptance Criteria
- Recurring expense rules include category, start date, amount, description, frequency, interval and optional end date.
- Supported frequencies are daily, weekly, monthly and yearly.
- Occurrence generation is deterministic and date-bounded.
- Inactive recurring rules produce no occurrences.
- Domain invariants reject invalid amounts, intervals, descriptions and date ranges.

## Status
Implemented
