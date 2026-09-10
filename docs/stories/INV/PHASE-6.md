# Phase 6 — Investments

## PW-INV-007 — Corporate actions
Purpose: represent security-level corporate actions without hiding balance-sheet changes inside imports. Supported v1 actions are stock splits, bonus issues, symbol changes and security renames. Acceptance: tenant ownership is enforced; effective date and positive ratios are mandatory; split/bonus exposes a deterministic quantity multiplier; symbol/name changes carry explicit new values; persistence has a source-controlled migration; tax-lot reconstruction remains deferred.

## PW-INV-008 — Investment imports
Purpose: validate broker/statement rows before canonical transaction creation. The import contract normalizes account number, symbol, date, transaction type, quantity, price, fees and currency. Acceptance: rows are processed deterministically by row number; invalid rows return explicit errors; negative money/quantity is rejected; buy/sell requires positive quantity; supported transaction types are explicit; no row is committed during validation.

## PW-INV-009 — Investment reconciliation
Purpose: compare expected ledger holdings with externally supplied holdings. Acceptance: union of account/security keys is compared; missing positions become zero on the absent side; quantity and cost-basis variances are explicit; configurable tolerances decide balanced/unbalanced; ordering is deterministic; no provider-specific reconciliation logic enters Domain.

## PW-INV-010 — Investment integration tests
Purpose: protect Phase 6 across persistence and business rules. Coverage includes investment entities, tenant-scoped persistence, weighted-average accounting, corporate-action ratio validation, import rejection and reconciliation variance. Release validation still requires local/CI build and test execution.
