# PW-ALERT-003 — Implement notification provider abstraction

## Goal
Keep notification delivery behind an Application-facing abstraction.

## Acceptance
- Application depends only on `INotificationProvider`.
- Provider implementations remain outside Domain/Application business rules.
- Notification payload carries tenant, severity, message and stable identity.

## Status
Implemented with a development provider in Infrastructure.
