# Personal Wealth Platform — Master Story Backlog

Ordered implementation contract. Sequential stories may be batched in one feature branch when they form a complete, testable checkpoint.

## Phases 0–5
- PW-CORE-001..007 [implemented]
- PW-DB-001..006 [implemented]
- PW-EVENT-001..006 [implemented]
- PW-DOC-001..012 [implemented]
- PW-BANK-001..008 [implemented]
- PW-EXP-001..006 [implemented]

## Phase 6 — Investments
- PW-INV-001..010 [implemented]

## Phase 7 — Assets
- PW-ASSET-001 Create physical/financial asset model [implemented]
- PW-ASSET-002 Implement asset valuation and ownership rules [implemented]
- PW-ASSET-003 Implement asset history/snapshots [implemented]
- PW-ASSET-004 Implement asset import workflow [implemented]
- PW-ASSET-005 Add asset tests [implemented]

## Phase 8 — Liabilities
- PW-LIAB-001 Create liability/loan model [implemented]
- PW-LIAB-002 Create repayment and schedule model [implemented]
- PW-LIAB-003 Implement interest/principal calculations [implemented]
- PW-LIAB-004 Implement credit-card liability workflow [implemented]
- PW-LIAB-005 Implement EPF/NPS liability/retirement-account modeling [implemented]
- PW-LIAB-006 Implement liability snapshots and ratios [implemented]
- PW-LIAB-007 Add liability tests [implemented]

## Phase 9 — Wealth Engine
- PW-WEALTH-001..008 [implemented]

## Phase 10 — API Foundation
- PW-API-001..006 [implemented]

## Phase 11 — Identity and Tenancy
- PW-AUTH-001..006 [implemented]

## Phase 12 — Angular Application
- PW-UI-001..008 [implemented]

## Phase 13 — Alerts and Notifications
- PW-ALERT-001..005 [pending]
## Phase 14 — AI Interpretation
- PW-AI-001..008 [pending]
## Phase 15 — Admin and Operations
- PW-ADMIN-001..004 [pending]
## Phase 16 — Deployment and Production Hardening
- PW-OPS-001..008 [pending]

## Global acceptance rules
Clean Architecture, SOLID, tenant isolation, deterministic financial calculations, secure configuration, testability and source-controlled EF migrations are mandatory. Future-phase business features are not silently implemented.
