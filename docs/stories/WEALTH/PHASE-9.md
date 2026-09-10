# Phase 9 — Wealth Engine

## PW-WEALTH-001 — Contracts
Define deterministic WealthInputs, WealthSnapshot and component/history contracts independent of transport or persistence.

## PW-WEALTH-002 — Assets/liabilities
Total assets combine cash, investments and other asset value. Total liabilities come from outstanding balances. Net worth is assets minus liabilities.

## PW-WEALTH-003 — Growth
Historical points are ordered deterministically and growth percentage is calculated against the previous value with zero-baseline protection.

## PW-WEALTH-004 — Allocation/concentration
Allocation groups wealth components and returns value/percentage. Concentration reports the share represented by the largest N components.

## PW-WEALTH-005 — Cash flow/savings
Cash flow equals income minus expenses. Savings rate is cash flow divided by income, with zero-income protection.

## PW-WEALTH-006 — Debt/liquidity
Debt-to-asset ratio uses liabilities/assets. Liquidity ratio uses liquid assets/liabilities, with zero-denominator protection.

## PW-WEALTH-007 — Historical snapshots
History remains immutable input data and can later be persisted as a snapshot table without changing the calculation contract.

## PW-WEALTH-008 — Tests
Deterministic tests cover all ratios, zero denominators, negative net worth, allocation, concentration, growth and ordering.
