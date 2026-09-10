# PW-EVENT-006 — Implement idempotency and processed-event handling

## Goal
Prevent duplicate event processing by tracking durable event identity after successful handler execution.

## Acceptance Criteria
- Processed event identity is unique and durable.
- Duplicate delivery is detected before handlers execute.
- A failed handler does not mark the event processed.
- Processing state is persisted through Infrastructure and is not coupled to a broker.

## Status
Implemented

## Validation
The implementation is covered by the event-bus contract and persistence model; SQL Server integration validation requires a working test SQL Server instance.
