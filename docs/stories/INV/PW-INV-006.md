# PW-INV-006 — Implement valuation and gain/loss calculations

## Goal
Calculate portfolio valuation and unrealized gain/loss deterministically from holdings and an approved price snapshot.

## Acceptance Criteria
- Market value equals holding quantity multiplied by the supplied market price.
- Unrealized gain/loss equals market value less cost basis.
- Percentage gain/loss is deterministic and returns zero when cost basis is zero.
- Price selection is deterministic when multiple dated prices are supplied for a security.
- Missing prices for open positions fail explicitly.
- Financial outputs use four-decimal banker's rounding.
- The valuation service has no direct dependency on a market-data SDK, EF Core or API.

## Status
Implemented
