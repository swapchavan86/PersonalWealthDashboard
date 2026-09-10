# PW-INV-002 — Create security/instrument master model

## Goal
Model tenant-owned investment instruments with stable identifiers and classification.

## Acceptance Criteria
- Symbol, name, security type and trading currency are represented.
- ISIN is supported as an optional external identifier.
- Security symbols are unique within a tenant.
- Security lifecycle supports active/inactive state without deleting historical references.
- Domain validation rejects missing symbol, name or currency.
- EF Core mappings and indexes are defined without leaking persistence concerns into Domain.

## Status
Implemented
