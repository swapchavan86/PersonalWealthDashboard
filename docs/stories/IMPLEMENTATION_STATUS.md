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

Expense story specifications are maintained under `docs/stories/EXP/`.

## Phase 6 — Investments

### Implemented

- `PW-INV-001` — Investment account/portfolio boundary and tenant-aware persistence model.
- `PW-INV-002` — Security/instrument master with symbol, classification, currency and optional ISIN.
- `PW-INV-003` — Investment transaction and holding model with financial precision and invariants.
- `PW-INV-004` — Deterministic portfolio replay using transaction-date/identity ordering, weighted-average cost, realized gain/loss, dividend income and explicit fees.
- `PW-INV-005` — Application-owned market-price provider abstraction. No external provider SDK is coupled to the core.
- `PW-INV-006` — Deterministic portfolio valuation and unrealized gain/loss from supplied market-price snapshots.

Implementation artifacts include:

- Domain investment entities under `src/PersonalWealth.Domain/Investments/`.
- Application portfolio and valuation engines under `src/PersonalWealth.Application/Investments/`.
- EF Core mappings under `src/PersonalWealth.Infrastructure/Persistence/Configurations/InvestmentConfigurations.cs`.
- Investment DbSets registered in `PersonalWealthDbContext`.
- Source-controlled investment migration `20260910170000_Investments`.
- Unit coverage under `tests/PersonalWealth.UnitTests/Investments/InvestmentPortfolioTests.cs`.
- Accounting methodology recorded in `docs/adr/ADR-INV-001-Investment-Accounting-Methodology.md`.

### Pending

- `PW-INV-007` — Corporate-action handling foundation.
- `PW-INV-008` — Investment import templates.
- `PW-INV-009` — Investment reconciliation.
- `PW-INV-010` — Investment integration tests.

The remaining Investment stories are intentionally not implemented prematurely. Corporate actions, import contracts and reconciliation rules require their own explicit boundaries and test fixtures.

## Current validation note

The latest GitHub commit sequence implements the Investment milestone directly on `main`, but GitHub Actions does not currently provide a workflow result for the latest merge state. Therefore this document does not claim a green CI run. Local validation should run `dotnet build` followed by `dotnet test` against the current `main` state before treating the milestone as release-ready.

The previous Expense follow-up corrected `Recurring_expense_respects_end_date_and_inactive_state` by changing the fixture end date to `2026-01-31`, matching the implementation's inclusive end-date semantics.

## Remaining implementation order

1. Finish Phase 6 — Investments: PW-INV-007 through PW-INV-010.
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
