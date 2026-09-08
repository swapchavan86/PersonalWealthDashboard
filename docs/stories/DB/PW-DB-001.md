# PW-DB-001 — Configure SQL Server and environment configuration

## Goal
Establish SQL Server configuration without embedding secrets.

## Acceptance Criteria
- Local development can provide a SQL Server connection string through local-only configuration/environment variables.
- Committed configuration contains no credentials and uses placeholders where needed.
- Production values can be supplied through secure environment/secret-management configuration.
- Configuration is consumed through appropriate options/configuration abstractions.
- Missing required configuration fails safely and clearly.

## Constraints
No business entities yet. Never commit secrets. Comments minimal and English-only.

## Validation
Build, tests and a configuration test proving no secret is required from committed files.

## Status
Ready
