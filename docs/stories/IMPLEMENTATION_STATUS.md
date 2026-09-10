# Story Implementation Status

This document is the quick-reference status map for the implementation backlog. `MASTER_BACKLOG.md` is the ordered backlog. Phase directories under `docs/stories/` contain the canonical per-story specifications where available.

## Completed phases

### Phase 0 — Engineering Foundation
PW-CORE-001 through PW-CORE-007 are completed.

### Phase 1 — Database Foundation
PW-DB-001 through PW-DB-006 are completed.

### Phase 2 — Events and Reliability
PW-EVENT-001 through PW-EVENT-006 are completed.

### Phase 3 — Documents and Deterministic Import
PW-DOC-001 through PW-DOC-012 are completed.

### Phase 4 — Banking
PW-BANK-001 through PW-BANK-008 are completed.

### Phase 5 — Expenses
PW-EXP-001 through PW-EXP-006 are implemented. The Expense phase includes category/expense domain models, application workflows, recurring rules, reporting projections, bank-transaction linking and tests.

Expense story specifications are now separated under `docs/stories/EXP/`:
- `PW-EXP-001.md`
- `PW-EXP-002.md`
- `PW-EXP-003.md`
- `PW-EXP-004.md`
- `PW-EXP-005.md`
- `PW-EXP-006.md`

## Current test fix

The reported local test failure was `Recurring_expense_respects_end_date_and_inactive_state`: the test supplied an end date of `2026-02-01` while asserting that only the `2026-01-01` occurrence exists. The recurring-expense implementation treats the configured end date as inclusive, so the test data was inconsistent with that contract.

The test now uses `2026-01-31`, preserving inclusive end-date semantics and verifying that the January occurrence is included while the next monthly occurrence is outside the rule's range. The inactive-state assertion remains unchanged.

## Remaining implementation order

1. Phase 6 — Investments: PW-INV-001 through PW-INV-010.
2. Phase 7 — Assets: PW-ASSET-001 through PW-ASSET-005.
3. Phase 8 — Liabilities: PW-LIAB-001 through PW-LIAB-007.
4. Phase 9 — Wealth Engine: PW-WEALTH-001 through PW-WEALTH-008.
5. Phase 10 — API Foundation: PW-API-001 through PW-API-006, including the versioned API surface needed for Swagger/Postman testing.
6. Phase 11 — Identity and Tenancy: PW-AUTH-001 through PW-AUTH-006.
7. Phase 12 — Angular Application: PW-UI-001 through PW-UI-008.
8. Phase 13 — Alerts and Notifications: PW-ALERT-001 through PW-ALERT-005.
9. Phase 14 — AI Interpretation: PW-AI-001 through PW-AI-008.
10. Phase 15 — Admin and Operations: PW-ADMIN-001 through PW-ADMIN-004.
11. Phase 16 — Deployment and Production Hardening: PW-OPS-001 through PW-OPS-008.

Stories should continue in backlog order unless an ADR explicitly changes the dependency. Sequential stories may be batched into one branch/PR when they form a complete, testable checkpoint.

## Validation status

The user-reported run reached successful compilation of the Domain, Contracts, Application, Infrastructure, Worker, API and test projects. Before this follow-up change, 60 unit tests passed and one recurring-expense unit test failed; 20 integration tests and 17 architecture tests passed. The corrected test has not been executed by this GitHub-only workflow, so the branch should be validated locally with `dotnet build` and `dotnet test` before merge.
