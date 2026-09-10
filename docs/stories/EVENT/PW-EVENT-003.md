# PW-EVENT-003 — Register event handlers and dispatching rules

## Goal
Standardize explicit handler registration and dispatching rules.

## Acceptance Criteria
- Registration is explicit and testable.
- Multiple handlers dispatch deterministically.
- Duplicate handler registration is rejected clearly.
- No hidden global mutable state or assembly scanning is introduced.

## Status
Implemented

## Implementation and validation
- Added `EventHandlerRegistry` in Application; callers explicitly register typed handlers and create an `IEventBus`.
- Registration order is preserved, multiple handlers are supported, and the same handler instance cannot be registered twice for one event type.
- Added focused tests for explicit registration, deterministic dispatch, duplicate registration, and invalid input.
- Validation: `dotnet clean`, `dotnet restore`, `dotnet build`, and `dotnet test` passed. Final totals: 63 passed, 0 failed, 0 skipped.
