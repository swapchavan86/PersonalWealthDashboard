# PW-EXP-001 — Create expense/category domain model

## Goal
Model expense categories and canonical expenses.

## Acceptance Criteria
- Category hierarchy and active-state rules are represented.
- Canonical expenses carry tenant ownership, category, date, amount and description.
- Expense invariants are enforced in the Domain layer.
- Domain models remain independent of EF Core and infrastructure.

## Status
Implemented
