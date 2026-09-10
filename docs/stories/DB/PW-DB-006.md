# PW-DB-006 — Add database integration-test infrastructure

## Goal
Provide isolated and repeatable SQL Server integration tests.

## Acceptance Criteria
- Integration tests can initialize/reset an isolated test database.
- Test configuration uses local/test secrets only and never production credentials.
- Tests are repeatable in developer and CI environments.

## Status
Implemented

## Implementation and validation
- Added a test-only SQL Server fixture that reads `PW_TEST_CONNECTION_STRING` and never falls back to application or production connection configuration.
- Each fixture receives a unique database name, supports explicit reset through `EnsureDeleted`/`EnsureCreated`, and cleans up its isolated database on disposal.
- Added focused tests for safe configuration, unique isolation, initialization, reset, and cleanup behavior.
- No production schema or migration changes were required.
- Validation: `dotnet clean`, `dotnet restore`, `dotnet build`, and `dotnet test` passed with LocalDB and the test-only connection-string environment variable. Final totals: 54 passed, 0 failed, 0 skipped.
