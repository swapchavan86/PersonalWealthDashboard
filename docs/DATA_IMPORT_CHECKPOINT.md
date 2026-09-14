# Unified Financial Data Import Checkpoint

The platform now has one Excel-compatible CSV template for the canonical financial dataset. The Imports screen can download the template and upload a completed file through `POST /api/v1/imports/wealth`.

Supported record types:

- BANK_ACCOUNT
- BANK_TRANSACTION
- EXPENSE
- INVESTMENT_ACCOUNT
- SECURITY
- INVESTMENT_TRANSACTION
- ASSET
- ASSET_VALUATION
- LIABILITY
- LIABILITY_REPAYMENT

The import is tenant-scoped, validates references, uses deterministic identifiers derived from `ExternalId`, applies existing domain constructors and investment/liability business rules, and runs inside a database transaction. Re-uploading the same ExternalId is intended to be idempotent.

The template is `docs/templates/PersonalWealth_Import_Template.csv`. It can be opened and edited in Excel and then uploaded unchanged as CSV.

Local validation:

1. Start SQL Server/LocalDB and apply EF migrations.
2. Start the API in Development mode.
3. Open Swagger or the Angular Imports page.
4. Download the template, add or edit rows, and upload it.
5. Verify the import result and query the database.
6. Call `/api/v1/wealth/dashboard` and verify totals against the source data.

The next checkpoint should add explicit golden-dataset calculation assertions across the dashboard and domain analytics before resuming AI implementation.
