# PW-AI-002 — Create deterministic financial context builders

Status: implemented

## Goal
Build explicit, minimized and auditable financial context for AI. AI has no unrestricted database access.

## Implementation
- Context DTOs contain only fields approved for the corresponding analysis surface.
- Builders serialize deterministic JSON and do not query persistence.
- Investment, expense and liability collections use stable ordering.
- Symbols are normalized and expense dates use a stable date-only representation.
- Tenant identifiers, account numbers and other infrastructure identifiers are not part of the AI context contracts.

## Acceptance
- Same input produces the same serialized context.
- Context construction is independently unit-testable.
- AI receives explicit context rather than database access.
