# PW-EXP-005 — Link bank transactions to expenses/categories

## Goal
Connect banking data to expense classification without unnecessary coupling between modules.

## Acceptance Criteria
- An expense can be linked to a canonical bank transaction through an Application abstraction.
- Linking is tenant-safe and verifies the referenced transaction belongs to the same tenant.
- A bank transaction cannot be linked to multiple expenses where the domain workflow requires uniqueness.
- Banking persistence details remain in Infrastructure.

## Status
Implemented
