# PW-DOC-011 — Implement first bank-statement document template

## Goal
Define the first real bank statement format as a deterministic, versioned import template before completing the bank business workflow.

## Acceptance Criteria
- Supported source format and template version are explicit.
- Expected columns/fields are documented.
- Transaction date parsing rules are explicit.
- Debit/credit/amount semantics are explicit.
- Balance/opening/closing semantics are explicit where supplied by the source.
- Account/institution identifiers are mapped safely.
- Text normalization rules are explicit.
- Blank rows, totals, headers and non-transaction rows are handled explicitly.
- Sample test fixtures contain no real credentials or sensitive personal data.
- Template tests cover valid and malformed rows.

## Architecture
The template is a document/schema concern. It must not contain banking business decisions such as expense categorization, transfer classification or wealth calculations.

## Status
Ready
