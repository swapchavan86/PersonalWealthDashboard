# Phase 8 — Liabilities

## PW-LIAB-001 — Liability/loan model
Model mortgages, personal loans, credit cards, education loans, EPF, NPS and other liability/retirement balances. Acceptance: currency, principal and start date are validated; outstanding balance starts at original principal; lifecycle is explicit.

## PW-LIAB-002 — Repayment/schedule
Repayments explicitly contain payment date, total, principal and interest. Loan schedules use deterministic monthly periods. Acceptance: repayment split is non-negative and cannot exceed total; schedule reduces balance and stops at zero.

## PW-LIAB-003 — Interest/principal
Monthly interest is calculated from outstanding principal and annual rate divided by twelve. Principal is payment less interest, capped by outstanding balance. Outputs use four-decimal banker's rounding.

## PW-LIAB-004 — Credit cards
Credit-card balances use the same liability boundary while allowing statement/repayment workflows to be linked to existing banking imports. No card provider SDK is part of Domain.

## PW-LIAB-005 — EPF/NPS
EPF and NPS are explicit liability types so retirement balances can be represented and later analysed with retirement-specific rules. Regulatory/tax calculations are intentionally deferred to dedicated stories.

## PW-LIAB-006 — Snapshots/ratios
Liability snapshots support historical balances. Wealth analytics consumes liability totals for debt-to-asset and liquidity ratios.

## PW-LIAB-007 — Tests
Tests cover validation, repayment split, interest calculation, balance reduction, zero-balance closure and EPF/NPS classification.
