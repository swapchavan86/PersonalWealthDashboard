# Unified Financial Data Import Checkpoint

The platform now uses a real Excel workbook for the canonical financial dataset. The Imports screen downloads `PersonalWealth_Import_Template.xlsx` with separate, clearly named sheets for each financial data type and an Instructions sheet.

Sheets:

- Bank Accounts
- Bank Transactions
- Expenses
- Investment Accounts
- Securities
- Investment Transactions
- Assets
- Asset Valuations
- Liabilities
- Liability Repayments
- Instructions

Each financial sheet has only the columns required for that data type. The API reads each sheet according to its own schema. Before database changes are made, the workbook is validated for required cells, numeric values, dates, and supported enum values. Validation failures identify the sheet and Excel cell address, for example `Sheet 'Assets', cell F4: AcquisitionValue must be a number`. A workbook with validation errors is rejected without importing partial data.

The legacy CSV format remains supported for compatibility. New users should use the Excel workbook.

The import is tenant-scoped, validates references, uses deterministic identifiers derived from `ExternalId`, applies existing domain constructors and investment/liability business rules, and runs inside a database transaction. Re-uploading the same workbook content remains idempotent through the content hash.

Local validation:

1. Start SQL Server/LocalDB and apply EF migrations.
2. Start the API in Development mode.
3. Open the Angular Imports page.
4. Download the Excel template and populate the relevant sheets with original financial values.
5. Upload the workbook.
6. If validation errors are shown, correct the listed sheet/cell values and upload again. No partial import is performed on validation failure.
7. After a successful import, verify database rows and call `/api/v1/wealth/dashboard` to reconcile totals against the source data.
8. Re-upload the exact same workbook and verify that no duplicate financial records are created.

UI visual redesign is intentionally deferred until the functional data/import/calculation checkpoints are complete. The next functional checkpoint is a golden dataset with explicit calculation assertions across dashboard and domain analytics before resuming AI implementation.
