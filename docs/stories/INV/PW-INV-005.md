# PW-INV-005 — Implement market-price provider abstraction

## Goal
Define an Application-owned market-price contract so external market-data providers remain replaceable Infrastructure adapters.

## Acceptance Criteria
- Application exposes a market-price provider abstraction and normalized price model.
- The abstraction accepts a set of security identifiers and an as-of date.
- Provider-specific SDKs and transport concerns are excluded from Domain and Application.
- Valuation can consume supplied normalized prices without contacting an external provider.
- Missing or invalid prices produce explicit failures rather than silent zero valuation.

## Status
Implemented — abstraction only. No external market-data provider is intentionally coupled to the platform at this stage.
