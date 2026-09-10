# PW-EVENT-001 — Create event bus abstraction

## Goal
Define application-facing event dispatch contracts.

## Acceptance Criteria
- Application and Domain do not depend on a concrete event bus.
- Event contracts are framework-independent.
- The abstraction is appropriate for the modular monolith.
- No concrete infrastructure implementation is introduced prematurely.

## Status
Implemented

## Implementation and validation
- Added Application-owned `IEventBus` and generic `IEventHandler<TEvent>` contracts using the existing Domain `IDomainEvent` metadata.
- No infrastructure, broker, DI registration, or persistence implementation was added; those concerns belong to later stories.
- Added focused contract tests proving the abstractions are implementable without framework or infrastructure types.
- Validation: `dotnet clean`, `dotnet restore`, `dotnet build`, and `dotnet test` passed. Final totals: 56 passed, 0 failed, 0 skipped.
