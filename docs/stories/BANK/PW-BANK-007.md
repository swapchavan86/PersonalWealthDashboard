# PW-BANK-007 — Implement bank import business workflow

## Goal
Connect the deterministic document pipeline to banking business processing.

## Acceptance Criteria
- Flow is scan -> identify template -> parse -> structural validation -> business validation -> stage -> duplicate detection -> canonical commit -> events.
- Application owns orchestration and business rules.
- Infrastructure owns file/database/provider implementations.
- Import is transactional and idempotent.
- Failed imports do not partially persist canonical financial data.
- AI is not required.

## Status
Ready
