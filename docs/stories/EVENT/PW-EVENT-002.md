# PW-EVENT-002 — Implement in-process event bus

## Goal
Dispatch application/domain events deterministically inside the modular monolith.

## Acceptance Criteria
- Handlers execute deterministically.
- Failure behavior is defined and test-covered.
- Cancellation is supported.
- No hidden global state or message broker is introduced.
- Transactional outbox behavior remains deferred to PW-EVENT-004.

## Status
Implemented

## Implementation and validation
- Added `InProcessEventBus` in Application with explicit handler delegates supplied at construction.
- Handlers execute sequentially in supplied order; the first failure propagates and stops later handlers; cancellation is checked between handlers.
- Added focused tests for ordering, failure semantics, cancellation, and no-handler behavior.
- No registration discovery, broker, worker, or outbox implementation was introduced.
- Validation: `dotnet clean`, `dotnet restore`, `dotnet build`, and `dotnet test` passed. Final totals: 60 passed, 0 failed, 0 skipped.
