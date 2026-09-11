# PW-ALERT-002 — Implement alert rules engine

## Goal
Evaluate deterministic financial thresholds against an explicit wealth context.

## Acceptance
- Supported rules cover net worth, cash, debt and savings rate.
- Only active definitions are evaluated.
- Evaluation output is deterministic and auditable.
- The engine contains no persistence or provider dependency.

## Status
Implemented in `PersonalWealth.Application.Alerts.AlertRuleEngine`.
