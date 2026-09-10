# PW-EVENT-004 — Implement transactional outbox

## Goal
Persist domain events atomically with business changes so successful persistence creates a durable publication record without requiring a broker or publisher worker.

## Acceptance Criteria
- Outbox records are committed in the same EF Core `SaveChanges` transaction as business changes.
- Event identity is durable and unique so the same event cannot create multiple outbox records.
- Outbox metadata includes event type, serialized payload, occurrence time, creation time, tenant context when applicable, publication state, and concurrency metadata.
- A failed duplicate outbox insert does not leave the associated business change committed.
- No publisher worker, broker integration, or processed-event/idempotency subsystem is introduced; those belong to later stories.

## Status
Implemented

## Implementation and validation
- Added the Application-owned `IOutbox` contract and Infrastructure `EfCoreOutbox` implementation.
- Added SQL Server outbox persistence with a unique `EventId` primary key, tenant metadata, publication state, and row-version concurrency metadata.
- Registered the outbox writer with Infrastructure dependency injection and kept it on the same `PersonalWealthDbContext` unit of work as business changes.
- Added a source-controlled EF Core migration and model snapshot for the outbox table.
- Added SQL Server integration tests covering atomic business/outbox commit and rollback on duplicate event identity.
- Validation requires a working local/test SQL Server instance through `PW_TEST_CONNECTION_STRING`; the current developer environment reported a LocalDB startup failure, so integration-test execution could not be completed in this environment.
