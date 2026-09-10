# ADR-INV-001 — Investment accounting and valuation methodology

## Status
Accepted for Phase 6 v1

## Context
The investment stories require deterministic holdings, cost basis, realized gain/loss and valuation. The backlog does not prescribe a specific accounting methodology.

## Decision
Phase 6 v1 uses weighted-average cost per tenant/account/security position.

- Buy cost basis = quantity × unit price + buy fees.
- Sell cost = quantity sold × current weighted-average cost.
- Net sell proceeds = quantity × unit price − sell fees.
- Realized gain/loss = net sell proceeds − cost of units sold.
- Remaining cost basis is reduced by the cost of units sold.
- Dividend income is tracked separately from capital gain/loss.
- Fees are explicitly tracked and are not silently embedded into unrelated metrics.
- Market value = open quantity × approved market price.
- Unrealized gain/loss = market value − remaining cost basis.
- Financial outputs are rounded to four decimal places using `MidpointRounding.ToEven`.

## Consequences
The methodology is deterministic, testable and suitable for replay from an ordered transaction ledger. FIFO, LIFO, tax-lot accounting, currency conversion and provider-specific tax treatment are intentionally outside Phase 6 v1 and require a future ADR before implementation.

## Microservice readiness
The accounting engine is implemented as pure Domain/Application behavior. It does not require EF Core, HTTP, a market-data SDK or API controllers. This keeps the business capability portable to a future Investment service without changing the financial rules.
