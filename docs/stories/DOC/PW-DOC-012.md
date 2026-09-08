# PW-DOC-012 — Implement bank document parsing and normalization adapter

## Goal
Read the first supported bank document and produce normalized statement records.

## Acceptance Criteria
- Parser is isolated behind the template abstraction.
- File-library details do not leak into Application/Domain business logic.
- Rows are normalized into a stable intermediate model.
- Malformed rows produce structured errors.
- Parser handles supported date, amount, debit/credit, description and balance semantics.
- No AI is required for parsing.
- No banking categorization, transfer detection or wealth calculation is implemented here.

## Status
Ready
