# Story Implementation Status

This document is the quick-reference status map for the implementation backlog. The authoritative story definitions remain in `MASTER_BACKLOG.md` and `STORY_SPECIFICATIONS.md`.

## Phase 5 — Expenses

The complete Expense phase is implemented in merged PR #31.

| Story | Status | Implementation checkpoint |
|---|---|---|
| PW-EXP-001 | Implemented | Expense category hierarchy and canonical expense domain model |
| PW-EXP-002 | Implemented | Application service workflows for expense/category create, update, get and list |
| PW-EXP-003 | Implemented | Recurring expense definitions, activation state and deterministic occurrence generation |
| PW-EXP-004 | Implemented | Tenant-safe category/month/trend expense reporting projections |
| PW-EXP-005 | Implemented | Tenant-safe bank transaction linking to expenses |
| PW-EXP-006 | Implemented | Unit coverage for category, recurrence, reporting and transaction-linking behavior |

## Current Build Fix

PR #31 introduced `RecurringExpense` with a private EF constructor. The constructor did not initialize the non-nullable `Description` property, which causes compiler error `CS8618` during a clean build.

The follow-up fix initializes `Description` to `string.Empty` in the private constructor. The public constructor continues to enforce the domain invariant that a recurring expense description must be non-empty.

## Story Discovery

Use these documents as follows:

- `MASTER_BACKLOG.md`: ordered phase/story list and implementation status.
- `STORY_SPECIFICATIONS.md`: story goals and acceptance contracts.
- `IMPLEMENTATION_STATUS.md`: concise implementation-to-story mapping for completed work.

The next unimplemented phase after Expense is Phase 6 — Investments (`PW-INV-001` through `PW-INV-010`). API/Swagger/Postman exposure remains a later Phase 10 concern unless the backlog is explicitly changed by an ADR or story decision.
