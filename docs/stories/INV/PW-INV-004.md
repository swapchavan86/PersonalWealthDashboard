# PW-INV-004 — Implement investment business rules

## Goal
Replay investment transactions into deterministic holdings and portfolio accounting results.

## Acceptance Criteria
- Transactions are replayed in deterministic transaction-date/identity order.
- Buys increase quantity and cost basis including buy fees.
- Sells reject quantities greater than the current holding.
- Sell cost is calculated using weighted-average cost and realized gain/loss is based on net proceeds.
- Dividend income is tracked separately from capital gain/loss.
- Fees are tracked explicitly.
- Cross-tenant transactions are rejected by the application portfolio engine.
- The engine is independent of EF Core, market-data providers and API concerns.

## Status
Implemented
