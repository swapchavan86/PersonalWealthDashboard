# PW-EVENT-005 — Implement outbox publisher worker

## Goal
Publish pending outbox events reliably through the existing event bus without coupling the worker to business handlers.

## Acceptance Criteria
- Pending messages are processed in deterministic creation order and bounded batches.
- Successful publication marks the message as published.
- Failures are persisted with attempt count, last error and a retry time; permanently exhausted messages remain observable.
- Worker uses scoped infrastructure dependencies and does not own business logic.
- Cancellation and transient failures are handled safely.

## Status
Implemented

## Validation
SQL Server integration validation requires a working local/test SQL Server instance through `PW_TEST_CONNECTION_STRING`.
