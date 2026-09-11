# PW-ALERT-001 — Create alert domain model

## Goal
Model tenant-owned financial alert definitions and lifecycle.

## Acceptance
- Alert name, condition, threshold, severity and lifecycle status are explicit.
- Alert definitions are tenant-owned.
- Invalid names and negative thresholds are rejected.
- Resolve, disable and activate transitions are explicit.

## Status
Implemented in `PersonalWealth.Domain.Alerts.AlertDefinition`.
