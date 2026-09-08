# PW-BANK-003 — Implement banking application interfaces and use cases

## Goal
Define Application-layer use cases and interfaces for banking operations.

## Acceptance Criteria
- Interfaces required by banking use cases live in Application unless genuinely domain-owned.
- Implementations are injected through dependency inversion.
- Application does not reference EF Core, SQL Server, filesystem or bank document libraries.
- SOLID principles are strongly followed, especially SRP, ISP and DIP.
- Use cases are independently unit-testable.

## Status
Ready
