# PW-DB-006 — Add database integration-test infrastructure

## Goal
Provide isolated and repeatable SQL Server integration tests.

## Acceptance Criteria
- Integration tests can initialize/reset an isolated test database.
- Test configuration uses local/test secrets only and never production credentials.
- Tests are repeatable in developer and CI environments.

## Status
Ready
