# PW-INV-001 — Create investment account/portfolio model

## Goal
Model tenant-owned investment accounts as the persistence and portfolio boundary for investment data.

## Acceptance Criteria
- Investment account identity, institution, account number, account type, currency and lifecycle status are represented.
- Tenant ownership is mandatory and enforced through the existing tenant-aware entity contract.
- Account numbers are unique within a tenant.
- Domain validation rejects missing institution, account number or currency.
- The Domain model remains independent of EF Core and Infrastructure.
- EF Core persistence mapping and a source-controlled migration are provided.

## Status
Implemented
