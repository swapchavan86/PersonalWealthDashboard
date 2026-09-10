# Phase 10 — API Foundation

## PW-API-001 — API conventions/versioning
All public endpoints use `/api/v1/...` route boundaries and transport DTOs rather than exposing domain entities as the intended contract.

## PW-API-002 — Validation/error handling
A centralized exception boundary maps argument/validation failures to Problem Details and unexpected failures to safe 500 responses. Controllers remain thin.

## PW-API-003 — Authentication boundary
JWT bearer is the production authentication boundary. Authority and audience are configuration-driven. Controllers can require authorization without embedding token logic.

## PW-API-004 — Banking APIs
Versioned read endpoints expose accounts and normalized transactions with bounded result sizes. Tenant filtering is inherited from persistence.

## PW-API-005 — Wealth/dashboard APIs
`GET /api/v1/wealth/dashboard` exposes cash, investments, other assets, liabilities and net worth through an Application-owned query boundary.

## PW-API-006 — Import/document APIs
Investment row validation is exposed through `POST /api/v1/imports/investments/validate`; validation never commits financial records.
