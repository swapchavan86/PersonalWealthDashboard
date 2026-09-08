# PW-BANK-006 — Implement reconciliation and statement balance rules

## Goal
Reconcile canonical transactions with statement opening/closing balances.

## Acceptance Criteria
- Opening and closing balance semantics are explicit.
- Transaction sums reconcile according to the supported statement format.
- Mismatches are reported rather than silently corrected.
- Rounding/decimal behavior is deterministic.

## Status
Ready
