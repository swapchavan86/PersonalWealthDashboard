# Phase 7 — Assets

## PW-ASSET-001 — Asset model
Tenant-owned assets represent property, vehicles, cash, precious metals, businesses and other wealth components. Identity includes name, type, currency, acquisition value/date and active state. Acceptance: non-empty identity/currency, non-negative acquisition value and valid acquisition date.

## PW-ASSET-002 — Valuation and ownership
Asset valuation is a separate historical record. Latest valuation is selected by valuation date and stable identity; acquisition value is the deterministic fallback. Acceptance: valuation cannot belong to another tenant and cannot reference an unknown asset.

## PW-ASSET-003 — History/snapshots
Every valuation is retained rather than overwritten. Acceptance: multiple dated observations are queryable; latest selection is deterministic; historical data remains auditable.

## PW-ASSET-004 — Import workflow
Asset imports use explicit rows and validation before persistence, following the existing deterministic document/import boundary. Acceptance: malformed names, currencies, dates and negative values are rejected; import identity can be used to make replays idempotent.

## PW-ASSET-005 — Tests
Tests cover construction invariants, latest valuation selection, acquisition fallback, tenant ownership and persistence smoke behavior. Financial values use decimal precision.
