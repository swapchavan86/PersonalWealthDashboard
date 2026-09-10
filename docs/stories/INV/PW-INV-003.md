# PW-INV-003 — Create holdings and transaction model

## Goal
Create the canonical investment transaction and current holding models for buys, sells, dividends and fees.

## Acceptance Criteria
- Transactions identify tenant, investment account, security, date, type, quantity, unit price, fees and currency.
- Buy and sell transactions require positive quantities.
- Quantity, price and fees are validated as non-negative where applicable.
- Gross transaction amount is deterministic and rounded to four decimal places using banker's rounding.
- Holdings track quantity and cost basis for an account/security pair.
- Holdings enforce no negative quantity and expose deterministic weighted-average cost.
- EF Core mappings use high precision for quantities and financial amounts.

## Status
Implemented
