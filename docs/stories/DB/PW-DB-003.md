# PW-DB-003 — Create initial database migration

## Goal
Create the first source-controlled EF Core Code First migration.

## Acceptance Criteria
- Migration creates the current schema from an empty database.
- Migration is reproducible and source-controlled.
- No manual database-first artifact is introduced.
- Migration does not contain secrets.

## Validation
Generate/apply migration against an isolated database and run tests.

## Status
Ready
