# PW-ALERT-005 — Add alert and notification tests

## Goal
Protect deterministic rules and delivery reliability with focused automated tests.

## Acceptance
- Rule thresholds and lifecycle filtering are covered.
- Delivery retries are covered.
- Successful notification idempotency is covered.
- No real notification provider or personal financial data is used in fixtures.

## Status
Implemented in `PersonalWealth.UnitTests/Alerts`.
